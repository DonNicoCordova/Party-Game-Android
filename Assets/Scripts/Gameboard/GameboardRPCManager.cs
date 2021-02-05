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
    public void GetNextPlayer()
    {
        if (GameManager.Instance.notActionTakenPlayers.Count == 0 && GameManager.Instance.GetActualPlayer() == null)
        {
            GameManager.Instance.SetActualPlayer(null);
            GameManager.Instance.roundFinished = true;
        }
        else
        {
            if (GameManager.Instance.GetActualPlayer() != null)
            {
                GameManager.Instance.actionTakenPlayers.Enqueue(GameManager.Instance.GetActualPlayer());
                GameManager.Instance.SetActualPlayer(null);
            }

            if (GameManager.Instance.notActionTakenPlayers.Count != 0)
            {
                GameManager.Instance.SetActualPlayer(GameManager.Instance.notActionTakenPlayers.Dequeue());
                GameManager.Instance.playersLadder.UpdateLadderInfo();
                GameManager.Instance.virtualCamera.Follow = GameManager.Instance.GetActualPlayer().transform;
                GameManager.Instance.virtualCamera.LookAt = GameManager.Instance.GetActualPlayer().transform;
            }
        }
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
    public void DebugMessage(string message, string player)
    {
        Debug.Log($"Player: {player} Message: {message}");
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
            Debug.Log($"UPDATING ENERGY TO PLAYER ID: {playerId} WITH: {newEnergy}");
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            player.playerStats.SetEnergyLeft(newEnergy);
            player.UpdateEnergy();
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
