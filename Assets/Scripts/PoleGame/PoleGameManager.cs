using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
namespace PoleMiniGame
{
    public class PoleGameManager : GenericPunSingletonClass<PoleGameManager>
    {
        [Header("Assets Config")]
        public TMPro.TextMeshProUGUI pointsText;
        public GameObject gameResultsCanvas;
        public GameObject guiCanvas;
        public GameObject instructionsCanvas;
        public PolePlayer leftCharacter;
        public Transform player1Spawn; 
        public GameLadder ladderController;

        public PolePlayer rightCharacter;
        public Transform player2Spawn;

        public ShieldCrystal leftCrystal;
        public ShieldCrystal rightCrystal;

        public FlyingHead flyingHead;
        public GameObject playerPrefab;
        public PolePlayerStats playerStats;
        public bool enabledToPlay = false;
        public TimerBar timerBar;

        public Queue<string> attackQueue = new Queue<string>();

        [Header("Game Configuration")]
        public DifficultyInfo[] difficulties;

        [Header("Pole Configuration")]
        public float defaultBlockCooldown = 2f;
        public int numberOfPieces;

        public event System.EventHandler UpdatedPoints;
        private int numberOfPlayers = 0;
        private List<PolePlayerStats> playersStats;
        [SerializeField]
        private float timeSpent = 0f;
        private float blockCooldown;
        private DifficultyInfo actualDifficulty;
        private void Start()
        {
            actualDifficulty = GetDifficultyInfo(Difficulty.Passive);
            playersStats = new List<PolePlayerStats>();
            blockCooldown = defaultBlockCooldown;
            UpdatePointsText();
            foreach (PlayerController player in GameManager.Instance.players)
            {
                PolePlayerStats newPlayerMinigameStat = new PolePlayerStats(player.playerStats.id, numberOfPieces);
                playersStats.Add(newPlayerMinigameStat);
            }
            LevelLoader.Instance.FadeIn();
            StartCoroutine(InitializeGuiProcess()); 
            StartCoroutine(processImInGame());
            Pole.Instance.InitializeWithRandomPieces(numberOfPieces);
            LevelLoader.Instance.FadeIn();
        }

        public void ResetStateOnPlayers()
        {
            foreach (PlayerController player in GameManager.Instance.players)
            {
                player.playerStats.currentMinigameStateFinished = false;
            }
        }
        private void Update()
        {
            if (enabledToPlay)
            {
                if (blockCooldown >= 0)
                {
                    blockCooldown -= Time.deltaTime;
                    blockCooldown = Mathf.Clamp(blockCooldown, 0, Mathf.Infinity);
                } 
                timeSpent += Time.deltaTime;
                playerStats.timeSpent = timeSpent; 
                if (attackQueue.Count > 0 && PhotonNetwork.IsMasterClient)
                {
                    string attackType = attackQueue.Dequeue();
                    if (attackType == "right")
                    {
                        photonView.RPC("BlockRightPlate", RpcTarget.All, GetMostPoints().playerId);
                    }
                    else
                    {
                        photonView.RPC("BlockLeftPlate", RpcTarget.All, GetMostPoints().playerId);
                    }
                }
            }
        }
        public void OnClickLeftButton()
        {
            leftCharacter.Attack();
        }
        public void OnClickRightButton()
        {
            rightCharacter.Attack();
        }
        public bool AllPlayersJoined() => numberOfPlayers == PhotonNetwork.PlayerList.Length;

        
        public void SpawnPlayer()
        {
            Debug.Log("SPAWNING PLAYER");
            Debug.Log($"PHOTON VIEW IS MINE: PLAYER {GameManager.Instance.GetMainPlayer().photonPlayer.NickName}");

            GameObject player1 = GameObject.Instantiate(playerPrefab, player1Spawn.position, player1Spawn.rotation);
            GameObject player2 = GameObject.Instantiate(playerPrefab, player2Spawn.position, player2Spawn.rotation);
            player2.transform.localScale = new Vector2(-player2.transform.localScale.x, player2.transform.localScale.y);
            PolePlayer player1Controller = player1.GetComponentInChildren<PolePlayer>();
            PolePlayer player2Controller = player2.GetComponentInChildren<PolePlayer>();
            leftCharacter = player1Controller;
            rightCharacter = player2Controller;
            Material newColor = GameManager.Instance.playerConfigs[GameManager.Instance.GetMainPlayer().photonPlayer.ActorNumber - 1].mainColor;
            player1Controller.SetColor(newColor);
            PolePlayerStats polePlayer = new PolePlayerStats(GameManager.Instance.GetMainPlayer().photonPlayer.ActorNumber, numberOfPieces);
            playerStats = polePlayer;
            player2Controller.SetColor(newColor);
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

        [PunRPC]
        private void ImInGame()
        {
            numberOfPlayers++;
            if (AllPlayersJoined())
            {
                SpawnPlayer();
            }
        }
        [PunRPC]
        public void SetCurrentState(string state)
        {
            PoleMiniGameSystem.Instance.SetState(state);
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
        [PunRPC]
        public void BlockPlayer(int playerId)
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
        [PunRPC]
        public void QueueAttack(string attack)
        {
            if (PhotonNetwork.IsMasterClient)
                attackQueue.Enqueue(attack);
        }
        [PunRPC]
        public void BlockLeftPlate(int playerId)
        {
            if (playerId == GameManager.Instance.GetMainPlayer().playerStats.id)
            {
                if (!BlockLeft())
                    this.photonView.RPC("QueueAttack", RpcTarget.MasterClient, "left");
            }
        }
        [PunRPC]
        public void BlockRightPlate(int playerId)
        {
            if (playerId == GameManager.Instance.GetMainPlayer().playerStats.id)
            {
                if (!BlockRight())
                    this.photonView.RPC("QueueAttack", RpcTarget.MasterClient, "right");
            }
        }
        [PunRPC]
        public void SpawnHead(int playerId)
        {
            if (playerId == GameManager.Instance.GetMainPlayer().playerStats.id)
            {
                EnableHead();
            }
        }
        public bool BlockRight()
        {
            if (!rightCrystal.IsEnabled() && blockCooldown == 0f)
            {
               blockCooldown = defaultBlockCooldown;
                rightCrystal.Enable();
                return true;
            } else
            {
                return false;
            }
        }
        public bool BlockLeft()
        {
            if (!leftCrystal.IsEnabled() && blockCooldown == 0f) 
            {
                blockCooldown = defaultBlockCooldown;

                leftCrystal.Enable();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool EnableHead()
        {
            if (!flyingHead.IsEnabled())
            {
                flyingHead.Enable();
                return true;
            }
            else
            {
                return false;
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
        public IEnumerator spawnHead()
        {
            while (enabledToPlay)
            {
                int playerToSpawn = Random.Range(0, GameManager.Instance.players.Count);
                while(playerToSpawn == GetMostPoints().playerId)
                {
                    playerToSpawn = Random.Range(0, GameManager.Instance.players.Count);
                }
                this.photonView.RPC("SpawnHead", RpcTarget.All, playerToSpawn);
                yield return new WaitForSeconds(actualDifficulty.attackCooldown);
            }
        }
        public IEnumerator InitializeGuiProcess()
        {
            while (!GameManager.Instance.AllPlayersJoined())
            {
                yield return new WaitForSeconds(0.5f);
            }
            ladderController.Initialize();
        }
        public bool AllPlayersDone()
        {
            return !playersStats.Exists(stats => stats.piecesLeft > 0);
        }
        public void InitializeGameOver()
        {
            PoleMinigameOverController gameOverController = gameResultsCanvas.GetComponent<PoleMinigameOverController>();
            gameOverController.Initialize();
        }
        public void ShowGameResults()
        {
            guiCanvas.SetActive(false);
            gameResultsCanvas.SetActive(true);
            timerBar.gameObject.SetActive(true);
        }
        public PolePlayerStats GetMostPoints() => playersStats.OrderBy(x => x.piecesLeft).First();
        public List<PolePlayerStats> GetWinners() => playersStats.OrderBy(x => x.timeSpent).ToList();
        public PolePlayerStats GetPlayerMinigameStats(int playerId) => playersStats.First(x => x.playerId == playerId);
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
        [PunRPC]
        public void ReducePoints(int playerId)
        {
            PolePlayerStats playerPoints = GetPlayerMinigameStats(playerId);
            playerPoints.piecesLeft  -= 1;
            if (GameManager.Instance.GetMainPlayer().playerStats.id == playerId)
            {
                playerPoints.timeSpent = playerStats.timeSpent;
                playerStats = playerPoints;
                UpdatePointsText();
            }
            UpdatePointsText();
            if (UpdatedPoints != null)
                UpdatedPoints(this, new System.EventArgs());
        }
        [PunRPC]
        public void UpdateTime(int playerId, float newTimeSpent)
        {
            PolePlayerStats playerPoints = GetPlayerMinigameStats(playerId);
            playerPoints.timeSpent = newTimeSpent;
            if (GameManager.Instance.GetMainPlayer().playerStats.id == playerId)
            {
                playerStats = playerPoints;
                UpdatePointsText();
            }
            UpdatePointsText();
        }
        private void UpdatePointsText()
        {
            pointsText.text = playerStats.piecesLeft.ToString();
        }
        public void EnableToPlay()
        {
            enabledToPlay = true;
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(spawnHead());
        }
    }
    [System.Serializable]
    public class PolePlayerStats
    {
        public int playerId;
        public int piecesLeft;
        public float timeSpent;

        public PolePlayerStats(int newPlayerId, int newPiecesLeft)
        {
            playerId = newPlayerId;
            piecesLeft = newPiecesLeft;
        }
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
        public float attackCooldown;
    }
}
