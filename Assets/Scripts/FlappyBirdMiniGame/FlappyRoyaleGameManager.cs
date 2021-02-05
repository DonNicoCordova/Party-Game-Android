using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
public class FlappyRoyaleGameManager : GenericPunSingletonClass<FlappyRoyaleGameManager>
{
    public int points = 0;
    public TimerBar timerBar;
    public GameObject instructionsCanvas;
    public GameObject gameResultsCanvas;
    public GameObject guiCanvas;

    // Start is called before the first frame update
    void Start()
    {
        LevelLoader.Instance.FadeIn();
        StartCoroutine(InitializeGuiProcess());
    }
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
    public void ResetStateOnPlayers()
    {
        foreach (PlayerController player in GameManager.Instance.players)
        {
            player.playerStats.currentMinigameStateFinished = false;
        }
    }
    public void ShowInstructions()
    {
        guiCanvas.SetActive(false);
        instructionsCanvas.SetActive(true);
        timerBar.gameObject.SetActive(true);
    }
    public void HideInstructions()
    {

        guiCanvas.SetActive(true);
        instructionsCanvas.SetActive(false);
        timerBar.gameObject.SetActive(false);
    }
    public void ShowGameResults()
    {
        guiCanvas.SetActive(false);
        gameResultsCanvas.SetActive(true);
        timerBar.gameObject.SetActive(true);
    }
    public System.Collections.IEnumerator InitializeGuiProcess()
    {
        while (!GameManager.Instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}