using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using System.IO;

public class GameManager : GenericSingletonClass<GameManager>
{
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    [Header("Dice")]
    public DiceController[] dicesInPlay;

    [Header("ThrowPlatform")]
    public BoxAccelController throwController;

    [Header("Graphical interfaces")]
    public TextMeshProUGUI shakesText;
    public GameObject phaseIndicator;
    public Color32 normalTextColor;
    public LadderController playersLadder;
    public GameObject joystick;
    public TimerBar timerBar;
    public GameOverController gameOverUI;
    public GameObject miniMap;


    [Header("Dice Locations")]
    public float lerpTime = 5.0f;
    public Transform diceOnCameraPosition1;
    public Transform diceOnCameraPosition2;

    [Header("Players Configuration")]
    public PlayerConfig[] playerConfigs;
    public string playerPrefab;
    public List<PlayerController> players;
    public int numberOfPlayers;

    [Header("Game Configuration")]
    public int maxRounds;

    [Header("Flags")]
    public Boolean diceOnDisplay = false;
    public Boolean playersOrdered = false;
    public Boolean roundFinished = false;
    public Queue<PlayerController> notActionTakenPlayers { get; private set; } = new Queue<PlayerController>();
    public Queue<PlayerController> actionTakenPlayers { get; private set; } = new Queue<PlayerController>();
    public List<Throw> roundThrows = new List<Throw>();

    public Boolean allReferencesReady = false;
    public Boolean allPlayersOnPosition = false;
    public Queue<MiniGameScene> miniGamesQueue = new Queue<MiniGameScene>();


    [SerializeField]
    private List<MiniGameScene> miniGamesPool = new List<MiniGameScene>();
    private SavedPlayersCollection savedPlayersCollection;
    private PlayerController mainPlayer;
    private PlayerController actualPlayer = null;
    private int round = -1;
    private TextMeshProUGUI phaseText;
    private Animator phaseAnimator;
    private Queue<string> messagesQueue = new Queue<string>();
    private Queue<string> statesQueue = new Queue<string>();
    private Queue<Command> _commands = new Queue<Command>();
    private Command _currentCommand;
    private void Start()
    {
        ConnectReferences();
        phaseText = phaseIndicator.GetComponentInChildren<TextMeshProUGUI>();
        phaseAnimator = phaseIndicator.GetComponent<Animator>();
        phaseAnimator.gameObject.SetActive(false);
        InitializeGUI();
        LevelLoader.Instance.FadeIn();
    }
    private void Update()
    {
        if (messagesQueue.Count > 0)
        {
            string message = messagesQueue.Dequeue();
            StartCoroutine(processShowMessage(message));
        }
        if (statesQueue.Count > 0)
        {
            string state = statesQueue.Dequeue();
            StartCoroutine(processChangeState(state));
        }
        if (Input.GetKeyDown("r"))
        {
            SavePlayers();
        }

        if (Input.GetKeyDown("t"))
        {
            LoadPlayers();
        }
        ProcessCommands();
    }
    public void ProcessCommands()
    {
        if (_currentCommand != null && _currentCommand.IsFinished == false)
            return;

        if (_commands.Any() == false)
            return;

        _currentCommand = _commands.Dequeue();
        _currentCommand.Execute();
        if (_currentCommand.Failed)
        {
            _currentCommand.Reset();
            _commands.Enqueue(_currentCommand);
            _currentCommand = null;
        }
    }
    public void SetMainPlayer(PlayerController newPlayer)
    {
        mainPlayer = newPlayer;
    }
    public void SpawnPlayer()
    {
        LocationController playerSpawn = playerConfigs[PhotonNetwork.LocalPlayer.ActorNumber - 1].startingLocation;
        Transform waypoint = playerSpawn.waypoint;
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab, waypoint.position, Quaternion.identity);
        PlayerController characterScript = playerObj.GetComponent<PlayerController>();
        characterScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        characterScript.playerStats.lastSpawnPosition = waypoint.transform;
        playerSpawn.photonView.RPC("SetOwner", RpcTarget.AllBuffered, characterScript.playerStats.id);
        //Initialize player
    }
    public void RespawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab, new Vector3(-2.69f, -22.47f, -10.68f), Quaternion.identity);
        PlayerController characterScript = playerObj.GetComponent<PlayerController>();
        characterScript.photonView.RPC("Resume", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
    public void RefreshPhaseAnimator()
    {
        phaseText = phaseIndicator.GetComponentInChildren<TextMeshProUGUI>();
        phaseAnimator = phaseIndicator.GetComponent<Animator>();
        phaseAnimator.gameObject.SetActive(false);
    }
    public void OrderPlayers()
    {
        List<Throw> orderedThrows = roundThrows.OrderByDescending(o => o.throwValue).ToList();
        int throwOrder = 1;
        foreach (Throw playerThrow in orderedThrows)
        {
            PlayerController throwPlayer = GetPlayer(playerThrow.playerId);
            throwPlayer.playerStats.throwOrder = throwOrder;
            notActionTakenPlayers.Enqueue(throwPlayer);
            throwOrder++;
        }
        playersOrdered = true;

        playersLadder.Initialize();
    }
    public void StartNextRound()
    {
        roundThrows.Clear();
        round++;
        roundFinished = false;
        ResetPlayers();
    }
    public PlayerController GetActualPlayer() => actualPlayer;
    public void SetActualPlayer(PlayerController nPlayer) => actualPlayer = nPlayer;
    public PlayerController GetMainPlayer() => mainPlayer;
    public bool ActualPlayerIsMainPlayer() => mainPlayer == actualPlayer;
    public bool DiceOnDisplay() => diceOnDisplay;
    public bool YourTurn() => diceOnDisplay && ActualPlayerIsMainPlayer();
    public void DisableJoystick()
    {
        joystick.SetActive(false);
    }
    public void EnableJoystick() => joystick.SetActive(true);
    public float GetRound() => round;
    public bool PlayersSetAndOrdered()
    {
        return notActionTakenPlayers.Count == numberOfPlayers && playersOrdered;
    }
    public bool RoundDone() => notActionTakenPlayers.Count == 0 && actionTakenPlayers.Count == numberOfPlayers && roundFinished;
    public bool NextRoundReady()
    {
        return notActionTakenPlayers.Count == numberOfPlayers && actionTakenPlayers.Count == 0;
    }
    public bool GameBoardDone()
    {
        return actionTakenPlayers.Count == numberOfPlayers && notActionTakenPlayers.Count == 0;
    }
    public void ShowMessage(string message)
    {
        StartCoroutine(processShowMessage(message));
    }
    public void SetCurrentState(string newState)
    {
        StartCoroutine(processChangeState(newState));
    }
    public void PlayerPass()
    {

        if (GameboardRPCManager.Instance.photonView.IsMine)
        {
            actualPlayer.playerStats.passed = true;
        }
    }
    public void SetMainPlayerEnergy(int energy)
    {
        GameboardRPCManager.Instance.photonView.RPC("UpdateEnergy", RpcTarget.Others, mainPlayer.playerStats.id, energy);
        mainPlayer.playerStats.SetEnergyLeft(energy);
    }
    private IEnumerator processShowMessage(string message)
    {
        if (phaseAnimator)
        {
            if (!phaseAnimator.gameObject.activeSelf)
            {
                phaseText.text = message;
                phaseAnimator.gameObject.SetActive(true);
                phaseAnimator.Play("PhaseInAnimation");
                yield return new WaitForSeconds(1.5f);
                phaseAnimator.SetTrigger("SlideOut");
                yield return new WaitForSeconds(1);
                phaseText.text = "";
                phaseAnimator.gameObject.SetActive(false);
            }
            else
            {
                messagesQueue.Enqueue(message);
            }
        }
        else
        {
            messagesQueue.Enqueue(message);
        }
    }
    private IEnumerator processChangeState(string newState)
    {
        yield return new WaitForSeconds(0.5f);
        if (GameboardRPCManager.Instance != null && GameboardRPCManager.Instance.photonView != null)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, newState);
        }
        else
        {
            statesQueue.Enqueue(newState);
        }
    }
    public void ResetStateOnPlayers()
    {
        foreach (PlayerController player in players)
        {
            player.playerStats.currentStateFinished = false;
            player.playerStats.currentMinigameStateFinished = false;
            player.playerStats.currentMinigameOver = false;
        }
    }
    public bool AllPlayersStateDone()
    {
        if (AllPlayersJoined())
        {
            foreach (PlayerController player in players)
            {
                if (!player.playerStats.currentStateFinished)
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
    public bool AllPlayersMinigameStateDone()
    {
        if (AllPlayersJoined())
        {
            foreach (PlayerController player in players)
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
    public bool AllPlayersMinigameOver()
    {
        if (AllPlayersJoined())
        {
            foreach (PlayerController player in players)
            {
                if (player.playerStats == null || !player.playerStats.currentMinigameOver)
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
    public PlayerController GetPlayer(int playerId)
    {
        bool playerExists = players.Any(x => x.playerStats.id == playerId);
        if (playerExists)
        {
            return players.First(x => x.playerStats.id == playerId);
        }
        else
        {
            return null;
        }
    }
    public bool AllPlayersJoined() => numberOfPlayers == PhotonNetwork.PlayerList.Length;
    public bool AllPlayersCharacterSpawned()
    {
        return GameObject.FindGameObjectsWithTag("Player").Length == numberOfPlayers;
    }
    public bool AllPlayersThrown()
    {
        return roundThrows.Count == players.Count;
    }
    public void ClearThrows()
    {
        roundThrows.Clear();
    }
    public void SetPlayersPlaces()
    {
        var orderedPlayers = players.OrderByDescending(player => player.playerStats.capturedZones.Count).ToList();
        for (int i = 0; i < orderedPlayers.Count; i++)
        {
            orderedPlayers[i].photonView.RPC("SetPlayerPlace", RpcTarget.All, i + 1);
        }
        playersLadder.photonView.RPC("UpdateLadderInfo", RpcTarget.All);
    }
    public void FinishGame()
    {
        HideMinimap();
        gameOverUI.Initialize();
    }
    public void ResetPlayers()
    {
        if (notActionTakenPlayers.Count > 0)
        {
            foreach (PlayerController player in notActionTakenPlayers)
            {
                player.ResetForNewRound();
            }
        }
        else if (actionTakenPlayers.Count > 0)
        {
            while (actionTakenPlayers.Count > 0)
            {
                PlayerController player = actionTakenPlayers.Dequeue();
                player.ResetForNewRound();
                notActionTakenPlayers.Enqueue(player);
            }
        }

    }
    public void ConnectReferences()
    {
        if (virtualCamera == null && GameObject.FindGameObjectWithTag("VirtualCamera") != null)
        {
            virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        }
        else if (GameObject.FindGameObjectWithTag("VirtualCamera") == null)
        {
            allReferencesReady = false;
            return;
        }
        if ((dicesInPlay == null || (dicesInPlay.Length > 0 && dicesInPlay[0] == null)) && GameObject.FindGameObjectsWithTag("Dice") != null)
        {
            GameObject[] taggedDices = GameObject.FindGameObjectsWithTag("Dice");
            DiceController[] newDicesInPlay = new DiceController[taggedDices.Length];
            int index = 0;
            foreach (GameObject dice in taggedDices)
            {
                DiceController diceController = dice.GetComponent<DiceController>();
                newDicesInPlay[index] = diceController;
                index++;
            }
            dicesInPlay = newDicesInPlay;
        }
        else if (GameObject.FindGameObjectWithTag("Dice") == null)
        {
            allReferencesReady = false;
            return;
        }
        if (throwController == null && GameObject.FindGameObjectWithTag("ThrowPlatform") != null)
        {
            throwController = GameObject.FindGameObjectWithTag("ThrowPlatform").GetComponent<BoxAccelController>();
        }
        else if (GameObject.FindGameObjectWithTag("ThrowPlatform") == null)
        {
            allReferencesReady = false;
            return;
        }
        if (phaseIndicator == null && GameObject.FindGameObjectWithTag("PhaseIndicator") != null)
        {
            phaseIndicator = GameObject.FindGameObjectWithTag("PhaseIndicator");
        }
        else if (GameObject.FindGameObjectWithTag("PhaseIndicator") == null)
        {

            allReferencesReady = false;
            return;
        }
        if (playersLadder == null && GameObject.FindGameObjectWithTag("PlayersLadder") != null)
        {
            playersLadder = GameObject.FindGameObjectWithTag("PlayersLadder").GetComponent<LadderController>();
        }
        else if (GameObject.FindGameObjectWithTag("PlayersLadder") == null)
        {

            allReferencesReady = false;
            return;
        }
        if (joystick == null && GameObject.FindGameObjectWithTag("Joystick") != null)
        {
            joystick = GameObject.FindGameObjectWithTag("Joystick");
        }
        else if (GameObject.FindGameObjectWithTag("Joystick") == null)
        {
            allReferencesReady = false;
            return;
        }
        if (timerBar == null && GameObject.FindGameObjectWithTag("TimeBar") != null)
        {
            timerBar = GameObject.FindGameObjectWithTag("TimeBar").GetComponent<TimerBar>();
        }
        else if (GameObject.FindGameObjectWithTag("TimeBar") == null)
        {
            allReferencesReady = false;
            return;
        }
        if (gameOverUI == null && GameObject.FindGameObjectWithTag("GameOverUI") != null)
        {
            gameOverUI = GameObject.FindGameObjectWithTag("GameOverUI").GetComponent<GameOverController>();
        }
        else if (GameObject.FindGameObjectWithTag("GameOverUI") == null)
        {
            allReferencesReady = false;
            return;
        }
        if (diceOnCameraPosition1 == null && GameObject.FindGameObjectWithTag("DiceDisplayOne") != null)
        {
            diceOnCameraPosition1 = GameObject.FindGameObjectWithTag("DiceDisplayOne").transform;
        }
        else if (GameObject.FindGameObjectWithTag("DiceDisplayOne") == null)
        {
            allReferencesReady = false;
            return;
        }
        if (diceOnCameraPosition2 == null && GameObject.FindGameObjectWithTag("DiceDisplayOne") != null)
        {
            diceOnCameraPosition2 = GameObject.FindGameObjectWithTag("DiceDisplayTwo").transform;
        }
        else if (GameObject.FindGameObjectWithTag("DiceDisplayTwo") == null)
        {
            allReferencesReady = false;
            return;
        }
        if (numberOfPlayers != players.Count)
        {
            allReferencesReady = false;
            return;
        }
        if (miniMap == null && GameObject.FindGameObjectWithTag("MiniMap") != null)
        {
            miniMap = GameObject.FindGameObjectWithTag("MiniMap");
        }
        else if (GameObject.FindGameObjectWithTag("MiniMap") == null)
        {
            allReferencesReady = false;
            return;
        }

        allReferencesReady = true;
    }
    public void InitializeGUI()
    {
        DisableJoystick();
        playersLadder.gameObject.SetActive(false);
        gameOverUI.gameObject.SetActive(false);
        SkillsUI.Instance.Initialize();
        SkillsUI.Instance.HideSkills();
    }
    public void ResumeGUI()
    {
        ResumeGUICommand resumeGUICommand = new ResumeGUICommand();
        _commands.Enqueue(resumeGUICommand);
    }
    public void SavePlayers()
    {
        SavedPlayersCollection newSavedCollection = new SavedPlayersCollection();
        newSavedCollection.savedPlayers = new PlayerStats[players.Count];
        newSavedCollection.capturedLocations = new CapturedLocation[players.Count];
        string jsonString = "{";
        string savedPlayersString = "\"savedPlayers\":[";
        string capturedLocationsString = "\"capturedLocations\":[";
        foreach (PlayerController player in players)
        {
            CapturedLocation playerCapturedLocations = new CapturedLocation();
            playerCapturedLocations.playerId = player.playerStats.id;
            string locationsNames = "";
            foreach (LocationController location in player.playerStats.capturedZones)
            {
                locationsNames += location.gameObject.name + ",";
            }
            locationsNames = locationsNames.Remove(locationsNames.Length - 1, 1);
            playerCapturedLocations.capturedLocations = locationsNames;
            savedPlayersString += JsonUtility.ToJson(player.playerStats) + ",";
            capturedLocationsString += JsonUtility.ToJson(playerCapturedLocations) + ",";
        }

        savedPlayersString = savedPlayersString.Remove(savedPlayersString.Length - 1, 1);
        capturedLocationsString = capturedLocationsString.Remove(capturedLocationsString.Length - 1, 1);
        capturedLocationsString += "]";
        savedPlayersString += "]";
        jsonString += savedPlayersString + ",";
        jsonString += capturedLocationsString;
        jsonString += "}";
        string path;
        if (File.Exists(Application.persistentDataPath + $"/SaveStates/"))
        {
            path = Application.persistentDataPath + $"/SaveStates/{PhotonNetwork.CurrentRoom.Name}.json";
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + $"/SaveStates/");
            path = Application.persistentDataPath + $"/SaveStates/{PhotonNetwork.CurrentRoom.Name}.json";
        }
        File.WriteAllText(path, jsonString);
    }
    public void LoadPlayers()
    {
        string path = Application.persistentDataPath + $"/SaveStates/{PhotonNetwork.CurrentRoom.Name}.json";
        string jsonString = File.ReadAllText(path);
        savedPlayersCollection = JsonUtility.FromJson<SavedPlayersCollection>(jsonString);
    }
    public PlayerStats GetSavedPlayerStats(int playerId) => savedPlayersCollection.savedPlayers.First(x => x.id == playerId);
    public CapturedLocation GetSavedCapturedLocation(int playerId) => savedPlayersCollection.capturedLocations.First(x => x.playerId == playerId);
    public void PopulateMinigamesForRound()
    {
        int index = 0;
        while (index < maxRounds && miniGamesPool.Any())
        {
            var rand = new System.Random();
            var pollIndex = rand.Next(miniGamesPool.Count);
            MiniGameScene miniGame = miniGamesPool[pollIndex];
            if (numberOfPlayers >= miniGame.minimumPlayers)
            {
                miniGamesQueue.Enqueue(miniGame);
                miniGamesPool.RemoveAt(pollIndex);
            }
            index++;
        }
    }
    public void HideMinimap()
    {
        if (miniMap.activeSelf)
        {
            miniMap.SetActive(false);
        }
    }
    public void ShowMinimap()
    {
        if (!miniMap.activeSelf)
        {
            miniMap.SetActive(true);
        }
    }
}
//These classes are for saving data over one scene to another

[Serializable]
public class SavedPlayersCollection
{
    [SerializeField]
    public PlayerStats[] savedPlayers;
    [SerializeField]
    public CapturedLocation[] capturedLocations;
}

[Serializable]
public class CapturedLocation
{
    [SerializeField]
    public int playerId;
    [SerializeField]
    public string capturedLocations;
}

[Serializable]
public class MiniGameScene
{
    [SerializeField]
    public string scene;
    [SerializeField]
    public int minimumPlayers;
}