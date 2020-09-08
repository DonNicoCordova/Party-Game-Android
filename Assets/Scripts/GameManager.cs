using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Dice")]
    public DiceController[] dicesInPlay;

    [Header("ThrowPlatform")]
    public BoxAccelController throwController;

    [Header("Graphical interfaces")]
    public TextMeshProUGUI shakesText;
    public TextMeshProUGUI throwText;
    public GameObject phaseIndicator;
    public GameObject damagePopPrefab;
    public Color32 normalTextColor;
    public Color32[] playerColors;
    [Header("Locations")]
    public float lerpTime = 5.0f;
    public Transform diceOnCameraPosition1;
    public Transform diceOnCameraPosition2;
    private bool allDiceStopped;


    private List<Throw> roundThrows = new List<Throw>();
    private Queue<PlayerStats> notActionTakenPlayers = new Queue<PlayerStats>();
    private Queue<PlayerStats> actionTakenPlayers = new Queue<PlayerStats>();
    private PlayerStats actualPlayer = null;
    private int round = 0; 
    private Boolean diceOnDisplay = false;
    private int numberOfPlayers;
    private TextMeshProUGUI phaseText;
    private Animator phaseAnimator;
    private Boolean playersOrdered = false;
    void OnEnable() => numberOfPlayers = PlayerPrefs.GetInt("players");
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        allDiceStopped = false;
        shakesText.text = throwController.maxShakes.ToString();

        phaseText = phaseIndicator.GetComponentInChildren<TextMeshProUGUI>();
        phaseAnimator = phaseIndicator.GetComponent<Animator>();
        phaseAnimator.gameObject.SetActive(false);

    }
    private void FixedUpdate()
    {
        bool tempValue = true;
        foreach(DiceController diceController in dicesInPlay)
        {
            if (diceController.GetVelocity() != Vector3.zero)
            {
                tempValue = false;
                break;
            }
        }
        allDiceStopped = tempValue;
    }
    public bool DicesStopped() => allDiceStopped;
    public void CreatePlayers()
    {
        if (notActionTakenPlayers.Count == 0)
        {
            UnityEngine.Debug.Log("CREATING PLAYERS");
            UnityEngine.Debug.Log(numberOfPlayers);
            while (notActionTakenPlayers.Count < numberOfPlayers)
            {
                PlayerStats playerStats = new PlayerStats();
                playerStats.id = notActionTakenPlayers.Count;
                playerStats.nickName = $"Jugador {notActionTakenPlayers.Count+1}";
                playerStats.mainColor = playerColors[notActionTakenPlayers.Count];
                notActionTakenPlayers.Enqueue(playerStats);
            }
        }

    }
    public void OrderPlayers()
    {
        while (roundThrows.Count < numberOfPlayers)
        {
            Throw newThrow = new Throw();
            roundThrows.Add(newThrow);
        }
        List<Throw> orderedThrows = roundThrows.OrderByDescending(o => o.GetValue()).ToList();
        foreach (Throw playerThrow in orderedThrows)
        {
            PlayerStats tempPlayer = notActionTakenPlayers.Dequeue();
            tempPlayer.nickName = playerThrow.playerNickname;
            tempPlayer.id = playerThrow.playerId;
            if (playerThrow.isMainPlayer)
                tempPlayer.isPlayer = true;
            notActionTakenPlayers.Enqueue(tempPlayer);
        }
        playersOrdered = true;
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
    }
    public void GetNextPlayer()
    {
        if (actualPlayer != null)
            actionTakenPlayers.Enqueue(actualPlayer);
        
        if (notActionTakenPlayers.Count == 0)
        {
            actualPlayer = null;
        }
        else
        {
            actualPlayer = notActionTakenPlayers.Dequeue();
        }
    }
    public bool NothingElseToDo() => (actualPlayer.moved && actualPlayer.usedSkill) || actualPlayer.passed;
    public bool DiceOnDisplay() => diceOnDisplay;
    public bool YourTurn() => diceOnDisplay && actualPlayer.isPlayer;
    public float GetRound() => round;
    public bool PlayersSetAndOrdered() => notActionTakenPlayers.Count == numberOfPlayers && playersOrdered;
    public bool NextRoundReady() => notActionTakenPlayers.Count == 0 && actionTakenPlayers.Count == numberOfPlayers;
    public void ShowMessage(string message) => StartCoroutine(processShowMessage(message));
    public void SetThrowText()
    {
        throwText.text = throwController.actualThrow.GetValue().ToString();
    }
    public void AddThrow(Throw newThrow) => roundThrows.Add(newThrow);
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
