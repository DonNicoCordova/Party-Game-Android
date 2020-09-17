using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BoxAccelController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuration")]
    public Transform[] dropPosition;
    public float diceRotationAcceleration = 10.0f;
    public float shakeDetectionThreshold;
    public float minShakeInterval = 1.0f;
    public float shakeForceMultiplier = 50f;
    public int maxShakes = 4;
    public Throw actualThrow;
    
    private float sqrShakeDetectionThreshold;
    private float timeSinceLastShake;
    private Boolean throwFinished = false;

    private GameManager gameManager;
    private Vector2 prevPoint;
    private Vector2 newPoint;
    private Vector2 screenTravel;
    private int shakesLeft;
    private Animator boxAnimator;
    private bool allDiceStopped;
    private float dicesStillTime = 1f;
    public void OnPointerDown(PointerEventData data)
    {
        if (!throwFinished && DicesReadyToPlay())
        {
            prevPoint = data.position;
        }
    }
    public void OnDrag(PointerEventData data)
    {
        if (!throwFinished && DicesReadyToPlay())
        {
            newPoint = data.position;
            screenTravel = newPoint - prevPoint;
            _processSwipe(data.delta);
        }
    }
    public void OnPointerUp(PointerEventData data)
    {
        if (!throwFinished && DicesReadyToPlay())
        {
            foreach (DiceController die in gameManager.dicesInPlay)
            {
                die.Throw(diceRotationAcceleration, shakeForceMultiplier);
            }
        }
    }
    private void _processSwipe(Vector3 movementDirection)
    {
    }
    void Start()
    {
        sqrShakeDetectionThreshold = Mathf.Pow(shakeDetectionThreshold, 2);
        shakesLeft = maxShakes;
        allDiceStopped = false;
        gameManager = GameManager.instance;
        boxAnimator = gameObject.GetComponent<Animator>();
    }
    public void CheckInput()
    {
        if (!throwFinished && DicesReadyToPlay())
        {
            if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold && Time.unscaledTime >= timeSinceLastShake + minShakeInterval && shakesLeft > 0)
            {
                ShakeRigidBodies();
                timeSinceLastShake = Time.unscaledTime;
            }
            if (Input.GetMouseButtonUp(0))
            {
                DisableDicesAnimations();
                foreach (DiceController die in gameManager.dicesInPlay)
                {
                    die.Throw(diceRotationAcceleration, shakeForceMultiplier);
                }
            }
        }
    }
    public void CheckIfDicesStopped()
    {
        bool tempValue = true;
        foreach (DiceController diceController in gameManager.dicesInPlay)
        {
            if (diceController.GetVelocity().sqrMagnitude <= 0.0006)
            {
                dicesStillTime -= Time.deltaTime;
                dicesStillTime = Mathf.Clamp(dicesStillTime, 0f, Mathf.Infinity);
            }
            else
            {
                dicesStillTime = 1f;
            }

            if (diceController.GetVelocity() != Vector3.zero && dicesStillTime != 0)
            {
                tempValue = false;
                break;
            }
        }
        allDiceStopped = tempValue;
    }

    public bool DicesStopped() => allDiceStopped;
    public void ShakeRigidBodies()
    {
        if (!throwFinished && DicesReadyToPlay())
        {
            shakesLeft -= 1;
            GameManager.instance.shakesText.text = shakesLeft.ToString();
            for (int i = 0; i < gameManager.dicesInPlay.Length; i++)
            {
                gameManager.dicesInPlay[i].ShakeDice(diceRotationAcceleration, shakeForceMultiplier * 2);
            }
        }
    }
    public float GetShakesLeft() => shakesLeft;
    public bool DicesReadyToPlay() {
        foreach (DiceController die in gameManager.dicesInPlay)
        {
            if (die.AnimatorIsPlaying())
            {
                return false;
            }
        }
        return true;
    }
    public void FinishedThrow()
    {
        throwFinished = true;
    }
    public bool IsThrowFinished() {
        return throwFinished;
    }
    public void AnimateReadyToPlay() {
        EnableDicesAnimations();
        boxAnimator.ResetTrigger("MoveToScreen");
        boxAnimator.SetTrigger("MoveToPlayArea");
        foreach (DiceController die in gameManager.dicesInPlay)
        {
            die.MoveToPlayArea();
        }
    }
    public void AnimateFinishedThrow()
    {
        EnableDicesAnimations();
        boxAnimator.ResetTrigger("MoveToPlayArea");
        boxAnimator.SetTrigger("MoveToScreen");
        foreach (DiceController die in gameManager.dicesInPlay)
        {
            die.MoveToScreen();
        }
    }
    public void DisableDicesAnimations()
    {
        foreach (DiceController die in gameManager.dicesInPlay)
        {
            die.DisableAnimator();
        }
    }
    public void EnableDicesAnimations()
    {
        foreach (DiceController die in gameManager.dicesInPlay)
        {
            die.EnableAnimator();
        }
    }
    public void Initialize()
    {
        throwFinished = false;
        actualThrow = null;
    }
}
