using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Dice")]
    public DiceController[] dicesInPlay;

    [Header("ThrowPlatform")]
    public BoxAccelController throwController;

    [Header("Graphical interfaces")]
    public TextMeshProUGUI shakesText;
    public GameObject phaseIndicator;
    public GameObject damagePopPrefab;
    public Color32 normalTextColor;
    public Color32[] playerColors;
    [Header("Locations")]
    public float lerpTime = 5.0f;
    public Transform diceOnCameraPosition1;
    public Transform diceOnCameraPosition2;
    private bool allDiceStopped;
    private Rigidbody dice1Rigidbody;
    private Rigidbody dice2Rigidbody;


    private Throw[] throws;
    private Queue<PlayerStats> notActionTakenPlayers = new Queue<PlayerStats>();
    private Queue<PlayerStats> actionTakenPlayers = new Queue<PlayerStats>();
    private PlayerStats actualPlayer = null;
    private int round = 0; 
    private Boolean diceOnDisplay = false;
    private int numberOfPlayers;
    private TextMeshProUGUI phaseText;
    private Animator phaseAnimator;
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
        dice1Rigidbody = dicesInPlay[0].GetComponent<Rigidbody>();
        dice2Rigidbody = dicesInPlay[1].GetComponent<Rigidbody>();

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

    public void ShowDicesOnCamera()
    {
        MoveDie(dice1Rigidbody, diceOnCameraPosition1);
        MoveDie(dice2Rigidbody, diceOnCameraPosition2);
        
        if (Vector3.Distance(diceOnCameraPosition1.position, dice1Rigidbody.position) < Vector3.kEpsilon &&
            Vector3.Distance(diceOnCameraPosition2.position, dice2Rigidbody.position) < Vector3.kEpsilon)
        {
            diceOnDisplay = true;
        }
    }

    public void MoveDie(Rigidbody die, Transform moveTo)
    {
        UnityEngine.Debug.Log($"MOVING DIE FROM: {die.position} TO: {moveTo.position} die.position != moveTo.position: {die.position != moveTo.position}");
        if (die.position != moveTo.position)
        {
            if (die.useGravity)
            {
                die.useGravity = false;
                Collider boxCollider = die.GetComponent<Collider>();
                boxCollider.isTrigger = true;
            }
            die.rotation = Quaternion.Slerp(die.rotation, moveTo.rotation, lerpTime * Time.deltaTime);
            die.position = Vector3.Lerp(die.position, moveTo.position, lerpTime * Time.deltaTime);
            Transform diceTransform = die.GetComponent<Transform>();
            diceTransform.localScale = Vector3.Lerp(diceTransform.localScale, moveTo.localScale, lerpTime * Time.deltaTime);
        }
    }
    public void CreatePlayers()
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
    public bool PlayersSet()
    {
        UnityEngine.Debug.Log($"CALLING PLAYERSSET {notActionTakenPlayers.Count} == {numberOfPlayers}: {notActionTakenPlayers.Count == numberOfPlayers}");
        return notActionTakenPlayers.Count == numberOfPlayers;
    }
   public bool NextRoundReady() => notActionTakenPlayers.Count == 0 && actionTakenPlayers.Count == numberOfPlayers;
    public void ShowMessage(string message) => StartCoroutine(processShowMessage(message));
    private IEnumerator processShowMessage(string message)
    {
        phaseText.text = message;
        if (!phaseAnimator.gameObject.activeSelf)
        {
            phaseAnimator.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(2);
        phaseAnimator.SetTrigger("SlideOut");
        yield return new WaitForSeconds(2);
        phaseText.text = "";
        phaseAnimator.gameObject.SetActive(false);
    }
}
