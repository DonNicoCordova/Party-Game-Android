using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Dice")]
    public DiceController[] dicesInPlay;

    [Header("ThrowPlatform")]
    public BoxAccelController throwController;

    [Header("Graphical interfaces")]
    public TextMeshProUGUI shakesText;
    public GameObject damagePopPrefab;
    public Color32 normalTextColor;
    [Header("Locations")]
    public float lerpTime = 5.0f;
    public Transform diceOnCameraPosition1;
    public Transform diceOnCameraPosition2;
    public Transform diceOnCameraPosition3;
    private bool allDiceStopped;
    private Rigidbody dice1Rigidbody;
    private Rigidbody dice2Rigidbody;
    private Rigidbody dice3Rigidbody;

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
        dice3Rigidbody = dicesInPlay[2].GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool tempValue = true;
        foreach(DiceController diceController in dicesInPlay)
        {
            Debug.Log($"DICE VELOCITY: {diceController.GetVelocity()}");
            if (diceController.GetVelocity() != Vector3.zero)
            {
                tempValue = false;
                break;
            }
        }
        Debug.Log($"SETTING ALLDICESTOPPED: {tempValue}");
        allDiceStopped = tempValue;
    }

    public bool DicesStopped() => allDiceStopped;

    public void ShowDicesOnCamera()
    {
        MoveDie(dice1Rigidbody, diceOnCameraPosition1);
        MoveDie(dice2Rigidbody, diceOnCameraPosition2);
        MoveDie(dice3Rigidbody, diceOnCameraPosition3);
        if (dice1Rigidbody.position == diceOnCameraPosition1.position &&
            dice2Rigidbody.position == diceOnCameraPosition2.position && 
            dice3Rigidbody.position == diceOnCameraPosition3.position)
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
        }
    }

    public bool DiceOnDisplay() => diceOnDisplay;
}
