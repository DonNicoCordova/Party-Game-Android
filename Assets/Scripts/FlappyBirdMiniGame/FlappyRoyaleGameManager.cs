using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

public class FlappyRoyaleGameManager : GenericPunSingletonClass<FlappyRoyaleGameManager>
{
    public GameObject explosion;
    public float gravityFactor = 0.75f;
    public TimerBar timerBar;
    public GameObject instructionsCanvas;
    public GameObject gameResultsCanvas;
    public GameObject guiCanvas;
    private int numberOfPlayers = 0;

    [Header("Player")]
    public string spaceShipPrefab;
    public Transform spawnPoint;
    public SpaceShipController mainPlayer;

    private List<FlappyRoyaleStats> playersStats;
    private Vector3 originalGravity;
    // Start is called before the first frame update
    void Start()
    {
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Debug.Log($"GRAVITY {Physics.gravity}");
        originalGravity = Physics.gravity;
        Physics.gravity = originalGravity * gravityFactor;
        playersStats = new List<FlappyRoyaleStats>();
        foreach (PlayerController player in GameManager.Instance.players)
        {
            FlappyRoyaleStats newPlayerMinigameStat = new FlappyRoyaleStats();
            newPlayerMinigameStat.playerId = player.playerStats.id;
            newPlayerMinigameStat.timeAlive = 0f;
            playersStats.Add(newPlayerMinigameStat);
        }
        Debug.Log("FADE IN ON ROYALE GAME");
        LevelLoader.Instance.FadeIn();
        StartCoroutine(processImInGame());
        StartCoroutine(InitializeGuiProcess());
    }
    private void Update()
    {
    }
    public bool AllPlayersJoined() => numberOfPlayers == PhotonNetwork.PlayerList.Length;
    [PunRPC]
    private void ImInGame()
    {
        Debug.Log("CALLING IM IN GAME ON FLAPPY");
        numberOfPlayers++;
        if (AllPlayersJoined())
        {
            Debug.Log("SPAWNING PLAYER");
            SpawnPlayer();
        }
    }
    public void SpawnPlayer()
    {
        GameObject playerShip = PhotonNetwork.Instantiate(spaceShipPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"SPACESHIP SPAWNED {playerShip.name}");
        SpaceShipController spaceShip = playerShip.GetComponentInChildren<SpaceShipController>();
        Debug.Log($"SPACESHIP CONTROLLER {spaceShip.gameObject.name}");
        spaceShip.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
    [PunRPC]
    public void SetCurrentState(string state)
    {
        FlappyMinigameSystem.Instance.SetState(state);
    }
    [PunRPC]
    public void SetStateDone(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("SETTING STATE DONE FOR FLAPPY MINIGAME");
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (!player.playerStats.currentMinigameStateFinished)
            {
                Debug.Log($"SETTING MINIGAME STATE DONDE TO PLAYER {player.playerStats.nickName}");
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
        FlappyMiniGameOverController gameOverController = gameResultsCanvas.GetComponent<FlappyMiniGameOverController>();
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
    }
    public void HideInstructions()
    {

        guiCanvas.SetActive(true);
        instructionsCanvas.SetActive(false);
    }
    public void ShowGameResults()
    {
        guiCanvas.SetActive(false);
        gameResultsCanvas.SetActive(true);
    }
    public System.Collections.IEnumerator InitializeGuiProcess()
    {
        while (!GameManager.Instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public bool LastPlayerDied()
    {
        Debug.Log("CHECKING IF LAST PLAYER ALIVE");
        Debug.Log($"playersStats.FindAll(s => s.alive == true).ToList().Count <= 1 => {playersStats.FindAll(s => s.alive == true).ToList().Count} <= 1 BOOL {playersStats.FindAll(s => s.alive == true).ToList().Count <= 1}");
        return playersStats.FindAll(s => s.alive == true).ToList().Count == 0;
    }
    public IEnumerator processImInGame()
    {
        while (this.photonView == null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        this.photonView.RPC("ImInGame", RpcTarget.AllBuffered);

    }
    public FlappyRoyaleStats GetStats(int playerToGet)
    {
        bool playerExists = playersStats.Any(x => x.playerId == playerToGet);
        if (playerExists)
        {
            return playersStats.First(x => x.playerId == playerToGet);
        }
        else
        {
            return null;
        }
    }
    private void OnDestroy()
    {
        Physics.gravity = originalGravity;
    }
    public FlappyRoyaleStats GetWinner()
    {
        return playersStats.OrderByDescending(o => o.timeAlive).ToList()[0];
    }
}
public class FlappyRoyaleStats
{
    public int playerId;
    public float timeAlive;
    public bool alive = false;
}