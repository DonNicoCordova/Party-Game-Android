using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using TMPro;

public class FlappyRoyaleGameManager : GenericPunSingletonClass<FlappyRoyaleGameManager>
{
    public GameObject explosion;
    public float gravityFactor = 0.75f;
    public TimerBar timerBar;
    public GameObject instructionsCanvas;
    public GameObject gameResultsCanvas;
    public GameObject guiCanvas;
    public List<DifficultyInfo> difficulties;

    [Header("Player")]
    public string spaceShipPrefab;
    public Transform spawnPoint;
    public SpaceShipController mainPlayer;

    [Header("UI")]
    public TextMeshProUGUI countdownCounter;

    private int numberOfPlayers = 0;
    private List<FlappyRoyaleStats> playersStats;
    private Vector3 originalGravity;
    private List<Path> paths;
    public DifficultyInfo actualDifficulty;
    // Start is called before the first frame update
    void Start()
    {
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
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
        LevelLoader.Instance.FadeIn();
        StartCoroutine(processImInGame());
        StartCoroutine(InitializeGuiProcess());
        paths = GameObject.FindObjectsOfType<Path>().ToList();
        actualDifficulty = GetDifficultyInfo(Difficulty.Passive);
    }
    public DifficultyInfo GetDifficultyInfo(Difficulty difficulty)
    {
        bool skillExists = difficulties.Any(info => info.difficulty == difficulty);
        if (skillExists)
        {
            return difficulties.First(info => info.difficulty == difficulty);
        }
        else
        {
            return null;
        }
    }
    public void DifficultyUp()
    {
        switch (actualDifficulty.difficulty)
        {
            case Difficulty.Passive:
                actualDifficulty = GetDifficultyInfo(Difficulty.Easy);
                break;
            case Difficulty.Easy:
                actualDifficulty = GetDifficultyInfo(Difficulty.Medium);
                break;
            case Difficulty.Medium:
                actualDifficulty = GetDifficultyInfo(Difficulty.Hard);
                break;
        }
    }
    public bool AllPlayersJoined() => numberOfPlayers == PhotonNetwork.PlayerList.Length;
    [PunRPC]
    private void ImInGame()
    {
        numberOfPlayers++;
        if (AllPlayersJoined())
        {
            SpawnPlayer();
        }
    }
    public void SpawnPlayer()
    {
        GameObject playerShip = PhotonNetwork.Instantiate(spaceShipPrefab, spawnPoint.position, spawnPoint.rotation);
        SpaceShipController spaceShip = playerShip.GetComponentInChildren<SpaceShipController>();
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
    public IEnumerator InitializeGuiProcess()
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
    public List<FlappyRoyaleStats> GetWinners()
    {
        return playersStats.OrderByDescending(o => o.timeAlive).ToList();
    }

    public void RemoveBottomFences()
    {
        foreach (Path path in paths)
        {
            path.RemoveBotFence();
        }
    }
    public void RemoveTopFences()
    {
        foreach (Path path in paths)
        {
            path.RemoveTopFence();
        }
    }
}
public class FlappyRoyaleStats
{
    public int playerId;
    public float timeAlive;
    public bool alive = true;
}
[System.Serializable]
public enum Difficulty
{
    Passive, Easy, Medium, Hard
}
[System.Serializable]
public class DifficultyInfo
{
    [SerializeField]
    public Difficulty difficulty;
    [SerializeField]
    public float obstacleDelay;
    [SerializeField]
    public float singleObstacleChance;
    [SerializeField]
    public float wallObstacleChance;
    [SerializeField]
    public float israObstacleChance;
}