using UnityEngine;
using System.Linq;
using Photon.Pun;
using System.Collections;
using UnityEngine.AI;

public class GameboardRPCManager : GenericPunSingletonClass<GameboardRPCManager>
{
    public NavMeshSurface surface;
    private void Start()
    {
        if (surface == null)
        {
            surface = GetComponent<NavMeshSurface>();
        }
        GameManager.Instance.numberOfPlayers = 0;
        StartCoroutine(processImInGame());
    }
    [PunRPC]
    private void SetActualPlayer(int playerId)
    {
        if (playerId == -1)
        {
            GameManager.Instance.SetActualPlayer(null);
        }
        else
        {
            if (GameManager.Instance.GetActualPlayer() != null)
            {
                if (GameManager.Instance.ActualPlayerIsMainPlayer())
                {
                    PlayerController mainPlayer = GameManager.Instance.GetMainPlayer();
                    SkillsUI.Instance.DisableSkillsButton();
                    mainPlayer.character.Move(Vector3.zero);
                    mainPlayer.enabledToMove = false;
                }
            }
            PlayerController newActualPlayer = GameManager.Instance.GetPlayer(playerId);
            GameManager.Instance.SetActualPlayer(newActualPlayer);

            if (GameManager.Instance.ActualPlayerIsMainPlayer())
            {
                SkillsUI.Instance.EnableSkillsButton();
                newActualPlayer.enabledToMove = true;
                newActualPlayer.RunCheckingCoRoutine();
                newActualPlayer.buttonChecker.ShowButtons();
                GameManager.Instance.ShowMessage("¡Te toca!");
            }

            GameManager.Instance.playersLadder.UpdateLadderInfo();
            GameManager.Instance.virtualCamera.Follow = GameManager.Instance.GetActualPlayer().transform;
            GameManager.Instance.virtualCamera.LookAt = GameManager.Instance.GetActualPlayer().transform;

        }

    }
    [PunRPC]
    private void FinishRound()
    {
        GameManager.Instance.SetActualPlayer(null);
        GameManager.Instance.roundFinished = true;

    }
    [PunRPC]
    private void AddThrow(string newThrow)
    {
        Throw throwObj = JsonUtility.FromJson<Throw>(newThrow);
        GameManager.Instance.roundThrows.Add(throwObj);
    }
    [PunRPC]
    private void ImInGame()
    {
        GameManager.Instance.numberOfPlayers++;
        if (GameManager.Instance.AllPlayersJoined() && GameManager.Instance.GetRound() == -1)
        {
            GameManager.Instance.SpawnPlayer();
        } else if (GameManager.Instance.AllPlayersJoined() && GameManager.Instance.GetRound() >= 0)
        {
            GameManager.Instance.players.Clear();
            GameManager.Instance.RespawnPlayer();
        }
    }
    [PunRPC]
    public void SetCurrentState(string state)
    {
        GameSystem.Instance.SetState(state);
    }
    [PunRPC]
    public void DebugMessage(string message, PhotonMessageInfo info)
    {
        Debug.Log($"DEBUG MESSAGE FROM {info.Sender.NickName}: {message}");
    }
    [PunRPC]
    public void SetStateDone(int playerId, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (player && !player.playerStats.currentStateFinished)
            {
                player.playerStats.currentStateFinished = true;
            }
        }
    }

    [PunRPC]
    public void SetPlayersOnOrder(string playersOrder, PhotonMessageInfo info)
    {

        int throwOrder = 1;
        string[] players = playersOrder.Split(',');
        foreach (string player in players)
        {
            PlayerController throwPlayer = GameManager.Instance.GetPlayer(int.Parse(player));
            throwPlayer.playerStats.throwOrder = throwOrder;
            GameManager.Instance.notActionTakenPlayers.Enqueue(throwPlayer);
            throwOrder++;
        }
        GameManager.Instance.playersOrdered = true;

        GameManager.Instance.playersLadder.Initialize();
    }

    [PunRPC]
    public void UpdateEnergy(int playerId, int newEnergy)
    {
        if (GameManager.Instance.GetMainPlayer().playerStats.id != playerId)
        {
            GameManager.Instance.UpdatePlayerEnergy(playerId, newEnergy);
        }
    }
    public IEnumerator processImInGame()
    {
        while (this.photonView == null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        this.photonView.RPC("ImInGame", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void SetPlayerUsingSkill(int playerId, PhotonMessageInfo info)
    {
        PlayerController playerUsingRPC = GameManager.Instance.GetPlayer(info.Sender.ActorNumber);
        if (playerId != -1)
        {
            PlayerController playerUsingSkill = GameManager.Instance.GetPlayer(playerId);

            SkillsUI.Instance.playerUsingSkills = playerUsingSkill;
        } else
        {
            SkillsUI.Instance.playerUsingSkills = null;
        }

    }
}
