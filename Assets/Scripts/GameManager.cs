using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
public class GameManager : GenericPunSingletonClass<GameManager>
{
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    [Header("Dice")]
    public DiceController[] dicesInPlay;

    [Header("ThrowPlatform")]
    public BoxAccelController throwController;

    [Header("Graphical interfaces")]
    public TextMeshProUGUI shakesText;
    public TextMeshProUGUI throwText;
    public GameObject phaseIndicator;
    public Color32 normalTextColor;
    public LadderController playersLadder;
    public GameObject joystick;
    public TimerBar timerBar;
    public GameOverController gameOverUI;

    [Header("Dice Locations")]
    public float lerpTime = 5.0f;
    public Transform diceOnCameraPosition1;
    public Transform diceOnCameraPosition2;

    [Header("Players Configuration")]
    public PlayerConfig[] playerConfigs;
    public string playerPrefab;
    public List<PlayerController> players;

    [Header("Game Configuration")]
    public int maxRounds;

    public Queue<PlayerController> notActionTakenPlayers { get; private set; } = new Queue<PlayerController>();
    public Queue<PlayerController> actionTakenPlayers { get; private set; } = new Queue<PlayerController>();

    private PlayerController mainPlayer;
    [SerializeField]
    private List<Throw> roundThrows = new List<Throw>();
    private PlayerController actualPlayer = null;
    private int round = -1; 
    private int numberOfPlayers;
    private TextMeshProUGUI phaseText;
    private Animator phaseAnimator;
    private Boolean diceOnDisplay = false;
    private Boolean playersOrdered = false;
    private Boolean roundFinished = false;
    private Boolean allPlayersOnPosition = false;
    private Queue<string> messagesQueue = new Queue<string>();

    private void Start()
    {
        if (GameSystem.Instance == null || GameSystem.Instance.GetCurrentStateName() != "MinigamePhase")
            this.photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    override public void Awake()
    {
        //shakesText.text = throwController.maxShakes.ToString();
        base.Awake();
        phaseText = phaseIndicator.GetComponentInChildren<TextMeshProUGUI>();
        phaseAnimator = phaseIndicator.GetComponent<Animator>();
        phaseAnimator.gameObject.SetActive(false);
        LevelLoader.Instance.FadeIn();
    }
    private void Update()
    {
        if (messagesQueue.Count > 0)
        {
            string message = messagesQueue.Dequeue();
            StartCoroutine(processShowMessage(message));
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
        characterScript.playerStats.lastCapturedZone = playerSpawn;
        playerSpawn.photonView.RPC("SetOwner", RpcTarget.AllBuffered, characterScript.playerStats.id);
        //Initialize player
    }
    public void OrderPlayers()
    {
        List<Throw> orderedThrows = roundThrows.OrderByDescending(o => o.throwValue).ToList();
        foreach (Throw playerThrow in orderedThrows)
        {
            PlayerController throwPlayer = GetPlayer(playerThrow.playerId);
            notActionTakenPlayers.Enqueue(throwPlayer);
        }
        playersOrdered = true;

        playersLadder.Initialize();
    }
    public void StartNextRound() 
    { 
        roundThrows.Clear();
        round++;
        roundFinished = false;
    }
    [PunRPC]
    public void GetNextPlayer()
    {
        if (notActionTakenPlayers.Count == 0 && actualPlayer == null)
        {
            actualPlayer = null;
            roundFinished = true;
        }
        else
        {
            if (actualPlayer != null)
            {
                actionTakenPlayers.Enqueue(actualPlayer);
                actualPlayer = null;
            }
            if(notActionTakenPlayers.Count != 0)
            {
                actualPlayer = notActionTakenPlayers.Dequeue();
                playersLadder.UpdateLadderInfo();
                virtualCamera.Follow = actualPlayer.transform;
                virtualCamera.LookAt = actualPlayer.transform;
            }
        }
    }
    public PlayerController GetActualPlayer() => actualPlayer;
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
    public bool NextRoundReady() => notActionTakenPlayers.Count == numberOfPlayers && actionTakenPlayers.Count == 0;
    public void ShowMessage(string message)
    {
        StartCoroutine(processShowMessage(message));
    }
    public void SetThrowText()
    {
        throwText.text = mainPlayer.playerStats.MovesLeft().ToString();
    }
    [PunRPC]
    private void AddThrow(string newThrow)
    {
        Throw throwObj = JsonUtility.FromJson<Throw>(newThrow);
        roundThrows.Add(throwObj);
    }
    public void PlayerPass()
    {

        if (this.photonView.IsMine)
        {
            actualPlayer.playerStats.passed = true;
        }
    }
    public void SetMainPlayerMoves(int moves)
    {
        mainPlayer.playerStats.SetMovesLeft(moves);
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
            } else
            {
                messagesQueue.Enqueue(message);
            }
        } else
        {
            messagesQueue.Enqueue(message);
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
    public void ResetStateOnPlayers()
    {
        foreach (PlayerController player in players)
        {
            player.playerStats.currentStateFinished = false;
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
        } else
        {
            return false;
        }
    }
    public PlayerController GetPlayer(int playerId) => players.First(x => x.playerStats.id == playerId);
    public bool AllPlayersJoined() => numberOfPlayers == PhotonNetwork.PlayerList.Length;
    public bool AllPlayersCharacterSpawned() 
    {
        return GameObject.FindGameObjectsWithTag("Player").Length == numberOfPlayers;
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
            PlayerController player = GetPlayer(playerId);
            if (!player.playerStats.currentStateFinished)
            {
                player.playerStats.currentStateFinished = true;
            }
        }
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
            orderedPlayers[i].photonView.RPC("SetPlayerPlace", RpcTarget.All, i+1);
        }
        playersLadder.photonView.RPC("UpdateLadderInfo", RpcTarget.All);
    }
    public void FinishGame()
    {
        gameOverUI.Initialize();
    }
    public void ResetPlayers()
    {
        while (actionTakenPlayers.Count > 0)
        {
            PlayerController player = actionTakenPlayers.Dequeue();
            player.ResetForNewRound();
            notActionTakenPlayers.Enqueue(player);
        }
    }
 
}
