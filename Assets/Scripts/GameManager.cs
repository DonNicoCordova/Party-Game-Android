using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
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

    [Header("Dice Locations")]
    public float lerpTime = 5.0f;
    public Transform diceOnCameraPosition1;
    public Transform diceOnCameraPosition2;

    [Header("Players Configuration")]
    public PlayerConfig[] playerConfigs;
    public string playerPrefab;
    public List<PlayerController> players;
    private PlayerController mainPlayer;
    [SerializeField]
    private List<Throw> roundThrows = new List<Throw>();
    private Queue<PlayerController> notActionTakenPlayers = new Queue<PlayerController>();
    private Queue<PlayerController> actionTakenPlayers = new Queue<PlayerController>();
    private PlayerController actualPlayer = null;
    private int round = 0; 
    private int numberOfPlayers;
    private TextMeshProUGUI phaseText;
    private Animator phaseAnimator;
    private Boolean diceOnDisplay = false;
    private Boolean playersOrdered = false;
    private Boolean roundFinished = false;

    private void Start()
    {
        this.photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        //shakesText.text = throwController.maxShakes.ToString();
        phaseText = phaseIndicator.GetComponentInChildren<TextMeshProUGUI>();
        phaseAnimator = phaseIndicator.GetComponent<Animator>();
        phaseAnimator.gameObject.SetActive(false);

    }
    public void SetMainPlayer(PlayerController newPlayer)
    {
        mainPlayer = newPlayer;
    }
    public void SpawnPlayer()
    {
        Transform waypoint = playerConfigs[PhotonNetwork.LocalPlayer.ActorNumber-1].startingLocation.waypoint;
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab, waypoint.position, waypoint.rotation);
        PlayerController characterScript = playerObj.GetComponent<PlayerController>();

        //Initialize player
        characterScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

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
        while (actionTakenPlayers.Count > 0)
        {
            PlayerController player = actionTakenPlayers.Dequeue();
            player.playerStats.moved = false;
            player.playerStats.usedSkill = false;
            player.playerStats.passed = false;
            notActionTakenPlayers.Enqueue(player);
        }
        round++;
        roundFinished = false;
    }
    public void GetNextPlayer()
    {
        if (notActionTakenPlayers.Count == 0 && actualPlayer == null)
        {
            Debug.Log("CANT GET ANOTHER PLAYER. ROUND FINISHED");
            actualPlayer = null;
            roundFinished = true;
        }
        else
        {
            if (actualPlayer != null)
            {
                Debug.Log("CHANGING ACTUAL PLAYER");
                Debug.Log($"ENQUEUING {actualPlayer.photonPlayer.NickName} INTO ACTION TAKEN PLAYERS [{actionTakenPlayers.Count}]");
                actionTakenPlayers.Enqueue(actualPlayer);
                actualPlayer = null;
            }
            if(notActionTakenPlayers.Count != 0)
            {
                actualPlayer = notActionTakenPlayers.Dequeue();
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
    public void DisableJoystick() => joystick.SetActive(false);
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
        Debug.Log($"ADDING THROW: {newThrow}");
        Throw throwObj = JsonUtility.FromJson<Throw>(newThrow);
        roundThrows.Add(throwObj);
    }
    public void PlayerPass() => actualPlayer.playerStats.passed = true;
    public void SetMainPlayerMoves(int moves) => mainPlayer.playerStats.SetMaxMoves(moves);
    private IEnumerator processShowMessage(string message)
    {
        phaseText.text = message;
        if (!phaseAnimator.gameObject.activeSelf)
        {
            phaseAnimator.gameObject.SetActive(true);
        }
        phaseAnimator.Play("PhaseInAnimation");
        yield return new WaitForSeconds(1.5f);
        phaseAnimator.SetTrigger("SlideOut");
        yield return new WaitForSeconds(1);
        phaseText.text = "";
        phaseAnimator.gameObject.SetActive(false);
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
    [PunRPC]
    public void SetCurrentState(string state)
    {
        GameSystem.instance.SetState(state);
    }
    [PunRPC]
    public void DebugMessage(string message, string player)
    {
        Debug.Log($"Player: {player} Message: {message}");
    }
}
