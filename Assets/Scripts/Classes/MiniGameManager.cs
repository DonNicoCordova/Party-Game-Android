using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
public class MiniGameManager : GenericPunSingletonClass<MiniGameManager>
{
    public GameObject instructionsCanvas;
    public GameObject gameResultsCanvas;
    public GameObject guiCanvas;
    public bool AllPlayersStateDone()
    {
        if (GameManager.Instance.AllPlayersJoined())
        {
            foreach (PlayerController player in GameManager.Instance.players)
            {
                if (!player.playerStats.currentMinigameStateFinished)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    [PunRPC]
    public void SetCurrentState(string state)
    {
        MinigameSystem.Instance.SetState(state);
    }
    [PunRPC]
    public void SetStateDone(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (!player.playerStats.currentMinigameStateFinished)
            {
                player.playerStats.currentMinigameStateFinished = true;
            }
        }
    }
    [PunRPC]
    public void SetMinigameDone(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (!player.playerStats.currentMinigameOver)
            {
                player.playerStats.currentMinigameOver = true;
            }
        }
    }
    public void InitializeGameOver()
    {
        MinigameOverController gameOverController = gameResultsCanvas.GetComponent<MinigameOverController>();
        gameOverController.Initialize();
    }
}

public class PlayerMinigameStats
{
    public int playerId;
    public int points;
}