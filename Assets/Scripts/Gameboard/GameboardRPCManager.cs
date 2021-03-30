using UnityEngine;
using System.Linq;
using Photon.Pun;
using System.Collections;

public class GameboardRPCManager : GenericPunSingletonClass<GameboardRPCManager>
{
    private void Start()
    {
        GameManager.Instance.numberOfPlayers = 0;
        StartCoroutine(processImInGame());
    }
    [PunRPC]
    private void SetActualPlayer(int playerId)
    {
        if (GameManager.Instance.GetActualPlayer() != null)
        {
            if (GameManager.Instance.ActualPlayerIsMainPlayer())
            {
                SkillsUI.Instance.DisableSkillsButton();
            }
        }
        PlayerController newActualPlayer = GameManager.Instance.GetPlayer(playerId);
        GameManager.Instance.SetActualPlayer(newActualPlayer);
        
        if (GameManager.Instance.ActualPlayerIsMainPlayer())
        {
            SkillsUI.Instance.EnableSkillsButton();
            GameManager.Instance.ShowMessage("¡Te toca!");
        }

        GameManager.Instance.playersLadder.UpdateLadderInfo();
        GameManager.Instance.virtualCamera.Follow = GameManager.Instance.GetActualPlayer().transform;
        GameManager.Instance.virtualCamera.LookAt = GameManager.Instance.GetActualPlayer().transform;

    }
    [PunRPC]
    private void FinishRound()
    {
        Debug.Log("TRYING TO FINISH ROUND");
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
    public void SetStateDone(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (!player.playerStats.currentStateFinished)
            {
                player.playerStats.currentStateFinished = true;
            }
        }
    }

    [PunRPC]
    public void UpdateEnergy(int playerId, int newEnergy)
    {
        if (GameManager.Instance.GetMainPlayer().playerStats.id != playerId)
        {
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (player != null)
            {
                player.playerStats.SetEnergyLeft(newEnergy);
                player.UpdateEnergy();
            }
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
 
}
