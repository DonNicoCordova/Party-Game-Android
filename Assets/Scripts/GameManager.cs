using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Dice")]
    public DiceController[] dicesInPlay;

    [Header("ThrowPlatform")]
    public BoxAccelController throwController;

    [Header("Graphical interfaces")]
    public TextMeshProUGUI shakesText;
    public TextMeshProUGUI phaseIndicator;
    public GameObject damagePopPrefab;
    public Color32 normalTextColor;

    [Header("Locations")]
    public float lerpTime = 5.0f;
    public Transform diceOnCameraPosition1;
    public Transform diceOnCameraPosition2;
    private bool allDiceStopped;
    private Rigidbody dice1Rigidbody;
    private Rigidbody dice2Rigidbody;


    private Queue<PlayerStats> notActionTakenPlayers;
    private Queue<PlayerStats> actionTakenPlayers;
    private PlayerStats actualPlayer;
    private int round = 0; 
    private Boolean diceOnDisplay = false;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one gamemanager in scene!");
            return;
        }
        instance = this;
        allDiceStopped = false;
        shakesText.text = throwController.maxShakes.ToString();
        dice1Rigidbody = dicesInPlay[0].GetComponent<Rigidbody>();
        dice2Rigidbody = dicesInPlay[1].GetComponent<Rigidbody>();
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
    public bool DiceOnDisplay() => diceOnDisplay;

    public bool YourTurn() => diceOnDisplay && actualPlayer.isPlayer;
    public float GetRound() => round;
    public bool PlayersSet() => notActionTakenPlayers.Count > 0 && round == 0;

    public bool NextRoundReady() => notActionTakenPlayers.Count == 0 && actionTakenPlayers.Count > 0 ;
}
