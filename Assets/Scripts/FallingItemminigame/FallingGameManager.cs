using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
public class FallingGameManager : MonoBehaviourPunCallbacks
{
    public static FallingGameManager Instance;
    public TMPro.TextMeshProUGUI pointsText;
    public ItemSpawner itemSpawner;
    public FallingGameLadderController ladderController;
    public int points = 0;
    public Queue<string> attackQueue = new Queue<string>();
    public GameObject instructionsCanvas;
    public GameObject gameResultsCanvas;
    public GameObject guiCanvas;
    public TimerBar timerBar;
    public List<PlayerMinigameStats> playersMinigameStats;
    public event System.EventHandler UpdatedPoints;

    // Start is called before the first frame update
    void Start()
    {
        playersMinigameStats = new List<PlayerMinigameStats>();
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        UpdatePointsText();
        foreach(PlayerController player in GameManager.Instance.players)
        {
            PlayerMinigameStats newPlayerMinigameStat = new PlayerMinigameStats();
            newPlayerMinigameStat.playerId = player.playerStats.id;
            newPlayerMinigameStat.points = 0;
            newPlayerMinigameStat.playerSpawnerDone = false;
            playersMinigameStats.Add(newPlayerMinigameStat);
        }
        LevelLoader.Instance.FadeIn();
        StartCoroutine(InitializeGuiProcess());
    }
    private void Update()
    {
        if (attackQueue.Count > 0 && PhotonNetwork.IsMasterClient)
        {
            string attackType = attackQueue.Dequeue();
            if (attackType == "right")
            {
                photonView.RPC("FireRightCannon", RpcTarget.All, GetMostPoints().playerId);
            }
            else
            {
                photonView.RPC("FireLeftCannon", RpcTarget.All, GetMostPoints().playerId);
            }
        }
    }
    [PunRPC]
    public void AddPoints(int playerId)
    {
        PlayerMinigameStats playerMinigameStats = GetPlayerMinigameStats(playerId);
        playerMinigameStats.points += 2;
        if (GameManager.Instance.GetMainPlayer().playerStats.id == playerId)
        {
            points = playerMinigameStats.points;
            UpdatePointsText();
        }
        if (UpdatedPoints != null)
            UpdatedPoints(this, new System.EventArgs());
    }
    [PunRPC]
    public void ReducePoints(int playerId)
    {
        PlayerMinigameStats playerPoints = GetPlayerMinigameStats(playerId);
        playerPoints.points -= 1;
        if (GameManager.Instance.GetMainPlayer().playerStats.id == playerId)
        {
            points = playerPoints.points;
            UpdatePointsText();
        }
        UpdatePointsText();
        if (UpdatedPoints != null)
            UpdatedPoints(this, new System.EventArgs());
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
    public bool AllPlayersSpawnerDone()
    {
        if (GameManager.Instance.AllPlayersJoined())
        {
            foreach (PlayerMinigameStats playerMinigameStats in playersMinigameStats)
            {
                if (!playerMinigameStats.playerSpawnerDone)
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
    public PlayerMinigameStats GetPlayerMinigameStats(int playerId) => playersMinigameStats.First(x => x.playerId == playerId);
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
    [PunRPC]
    public void SetSpawnerDone(int playerId)
    {
        PlayerMinigameStats playerPoints = GetPlayerMinigameStats(playerId);
        playerPoints.playerSpawnerDone = true;
    }
    public PlayerMinigameStats GetMostPoints() => playersMinigameStats.OrderByDescending(x => x.points).First();
    public System.Collections.IEnumerator InitializeGuiProcess()
    {
        while (!GameManager.Instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
        ladderController.Initialize();
    }
}

public class PlayerMinigameStats
{
    public int playerId;
    public bool playerSpawnerDone = false;
    public int points;
}