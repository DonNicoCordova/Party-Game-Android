using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
public class FallingGameManager : MonoBehaviourPunCallbacks
{
    public static FallingGameManager Instance;
    public TMPro.TextMeshProUGUI pointsText;
    public ItemSpawner itemSpawner;
    public List<PlayerPoints> playersPoints = new List<PlayerPoints>();
    public FallingGameLadderController ladderController;
    public int points = 0;
    public Queue<string> attackQueue = new Queue<string>();
    public GameObject instructionsCanvas;
    public GameObject gameResultsCanvas;
    public GameObject guiCanvas;
    public TimerBar timerBar;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        UpdatePointsText();
        foreach(PlayerController player in GameManager.Instance.players)
        {
            PlayerPoints newPlayerPoint = new PlayerPoints();
            newPlayerPoint.playerId = player.playerStats.id;
            newPlayerPoint.points = 0;
            playersPoints.Add(newPlayerPoint);
        }
        ladderController.Initialize();
        LevelLoader.Instance.FadeIn();
    }
    private void Update()
    {
        if (attackQueue.Count > 0 && PhotonNetwork.IsMasterClient)
        {
            string attackType = attackQueue.Dequeue();
            if (attackType == "right")
            {
                FallingGameManager.Instance.photonView.RPC("FireRightCannon", RpcTarget.All, GetMostPoints().playerId);
            }
            else
            {
                FallingGameManager.Instance.photonView.RPC("FireLeftCannon", RpcTarget.All, GetMostPoints().playerId);
            }
        }
    }
    public void AddPoints(int newPoints)
    {
        PlayerController playerController = GameManager.Instance.GetMainPlayer();
        points += newPoints;
        PlayerPoints playerPoints = GetPlayerPoints(playerController.playerStats.id);
        playerPoints.points = points;
        UpdatePointsText();
        ladderController.photonView.RPC("UpdateLadderInfo", RpcTarget.All);
        this.photonView.RPC("UpdatePlayerPoints", RpcTarget.Others, playerController.playerStats.id, points);
    }
    public void ReducePoints(int newPoints)
    {
        PlayerController playerController = GameManager.Instance.GetMainPlayer();
        points -= newPoints;
        UpdatePointsText();
        PlayerPoints playerPoints = GetPlayerPoints(playerController.playerStats.id);
        playerPoints.points = points;
        UpdatePointsText();
        ladderController.photonView.RPC("UpdateLadderInfo", RpcTarget.All);
        this.photonView.RPC("UpdatePlayerPoints", RpcTarget.Others, playerController.playerStats.id, points);
    }
    private void UpdatePointsText()
    {
        pointsText.text = points.ToString();
    }

    [PunRPC]
    public void QueueAttack(string attack)
    {
        if (PhotonNetwork.IsMasterClient)
            attackQueue.Enqueue(attack);
    }

    [PunRPC]
    public void FireLeftCannon(int playerId)
    {
        if (playerId == GameManager.Instance.GetMainPlayer().playerStats.id)
        {
            if (!itemSpawner.FireLeftCannon())
                this.photonView.RPC("QueueAttack",RpcTarget.MasterClient,"left");
        }
    }
    [PunRPC]
    public void FireRightCannon(int playerId)
    {
        if (playerId == GameManager.Instance.GetMainPlayer().playerStats.id)
        {
            if (!itemSpawner.FireRightCannon())
                this.photonView.RPC("QueueAttack", RpcTarget.MasterClient, "right");
        }
    }
    [PunRPC]
    public void UpdatePlayerPoints(int playerId, int points)
    {
        PlayerPoints playerPoints = GetPlayerPoints(playerId);
        if (playerPoints != null)
        {
            playerPoints.points = points;
        }
    }
    [PunRPC]
    public void SetCurrentState(string state)
    {
        MinigameSystem.Instance.SetState(state);
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
    public PlayerPoints GetPlayerPoints(int playerId) => playersPoints.First(x => x.playerId == playerId);
    public PlayerPoints GetMostPoints() => playersPoints.OrderByDescending(x => x.points).First();
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
    public void InitializeGameOver()
    {
        MinigameOverController gameOverController = gameResultsCanvas.GetComponent<MinigameOverController>();
        gameOverController.Initialize();
    }
}

public class PlayerPoints
{
    public int playerId;
    public int points;
}
