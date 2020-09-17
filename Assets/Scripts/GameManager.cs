using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
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
    public PlayerColor[] playerConfigs;
    public GameObject playerPrefab;
    public List<PlayerStats> players;
    private PlayerStats mainPlayer;
    private List<Throw> roundThrows = new List<Throw>();
    private Queue<PlayerStats> notActionTakenPlayers = new Queue<PlayerStats>();
    private Queue<PlayerStats> actionTakenPlayers = new Queue<PlayerStats>();
    private PlayerStats actualPlayer = null;
    private int round = 0; 
    private int numberOfPlayers;
    private TextMeshProUGUI phaseText;
    private Animator phaseAnimator;
    private Boolean diceOnDisplay = false;
    private Boolean playersOrdered = false;
    private Boolean roundFinished = false;
    void OnEnable() => numberOfPlayers = PlayerPrefs.GetInt("players");
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
    public void CreatePlayers()
    {
        if (notActionTakenPlayers.Count == 0)
        {
            while (players.Count < numberOfPlayers)
            {
                PlayerStats playerStats = new PlayerStats();
                playerStats.id = players.Count;
                playerStats.nickName = $"Jugador {players.Count + 1}";
                playerStats.mainColor = playerConfigs[players.Count].mainColor;
                playerStats.orbColor = playerConfigs[players.Count].orbColor;
                Transform waypoint = playerConfigs[players.Count].startingLocation.waypoint;
                GameObject newCharacter = Instantiate<GameObject>(playerPrefab, waypoint.position, waypoint.rotation);
                playerStats.playableCharacter = newCharacter;
                PlayerGraficsController gfxController = newCharacter.GetComponentInChildren<PlayerGraficsController>();
                gfxController.ChangeMaterial(playerConfigs[players.Count].mainColor);
                CharacterMoveController moveController = playerStats.playableCharacter.GetComponent<CharacterMoveController>();
                if (players.Count == 0)
                {
                    playerStats.isPlayer = true;
                    mainPlayer = playerStats;
                    virtualCamera.Follow = newCharacter.transform;
                    virtualCamera.LookAt = newCharacter.transform;
                    moveController.joystick = joystick.GetComponent<FloatingJoystick>();
                }
                moveController.player = playerStats;
                players.Add(playerStats);
            }
        }

    }
    public void OrderPlayers()
    {
        //Creating throws for testing
        foreach (PlayerStats playerStats in players)
        {
            if (!playerStats.isPlayer)
            {
                Throw newThrow = new Throw(playerStats);
                roundThrows.Add(newThrow);
            }
        }
        List<Throw> orderedThrows = roundThrows.OrderByDescending(o => o.GetValue()).ToList();
        foreach (Throw playerThrow in orderedThrows)
        {
            notActionTakenPlayers.Enqueue(playerThrow.player);
        }
        playersOrdered = true;

        playersLadder.Initialize(notActionTakenPlayers.ToArray());
    }
    public void StartNextRound() 
    { 
        while (actionTakenPlayers.Count > 0)
        {
            PlayerStats playerStats = actionTakenPlayers.Dequeue();
            playerStats.moved = false;
            playerStats.usedSkill = false;
            playerStats.passed = false;
            notActionTakenPlayers.Enqueue(playerStats);
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
                Debug.Log($"ENQUEUING {actualPlayer.nickName} INTO ACTION TAKEN PLAYERS [{actionTakenPlayers.Count}]");
                actionTakenPlayers.Enqueue(actualPlayer);
                actualPlayer = null;
            }
            if(notActionTakenPlayers.Count != 0)
            {
                actualPlayer = notActionTakenPlayers.Dequeue();
                virtualCamera.Follow = actualPlayer.playableCharacter.transform;
                virtualCamera.LookAt = actualPlayer.playableCharacter.transform;
            }
        }
    }
    public PlayerStats GetActualPlayer() => actualPlayer;
    public PlayerStats GetMainPlayer() => mainPlayer;
    public bool DiceOnDisplay() => diceOnDisplay;
    public bool YourTurn() => diceOnDisplay && actualPlayer.isPlayer;
    public void DisableJoystick() => joystick.SetActive(false);
    public void EnableJoystick() => joystick.SetActive(true);
    public float GetRound() => round;
    public bool PlayersSetAndOrdered() => notActionTakenPlayers.Count == numberOfPlayers && playersOrdered;
    public bool RoundDone() => notActionTakenPlayers.Count == 0 && actionTakenPlayers.Count == numberOfPlayers && roundFinished;
    public bool NextRoundReady() => notActionTakenPlayers.Count == numberOfPlayers && actionTakenPlayers.Count == 0;
    public void ShowMessage(string message) => StartCoroutine(processShowMessage(message));
    public void SetThrowText()
    {
        throwText.text = mainPlayer.MovesLeft().ToString();
    }
    public void AddThrow(Throw newThrow) => roundThrows.Add(newThrow);
    public void PlayerPass() => actualPlayer.passed = true;
    public void SetMainPlayerMoves(int moves) => mainPlayer.SetMaxMoves(moves);
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
}
