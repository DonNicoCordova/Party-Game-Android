using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BoxAccelController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuration")]
    public Transform[] dropPosition;
    public float diceRotationAcceleration = 3.0f;
    public float shakeDetectionThreshold;
    public float minShakeInterval = 1.0f;
    public float shakeForceMultiplier = 50f;
    public int maxShakes = 4;
    private float sqrShakeDetectionThreshold;
    private float timeSinceLastShake;
    private Boolean dicesReadyToPlay = false;
    private Boolean throwFinished = false;

    private Throw actualThrow;
    private GameManager gameManager;
    private Vector2 prevPoint;
    private Vector2 newPoint;
    private Vector2 screenTravel;
    private int shakesLeft;
    private Animator boxAnimator;
    public void OnPointerDown(PointerEventData data)
    {
        if (!throwFinished && DicesReadyToPlay())
        {
            shakesLeft = maxShakes;
            GameManager.instance.shakesText.text = shakesLeft.ToString();
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
        if (dicesReadyToPlay)
            return true;
        foreach (DiceController die in gameManager.dicesInPlay)
        {
            if (die.AnimatorIsPlaying())
            {
                dicesReadyToPlay=false;
                return false;
            }
        }
        dicesReadyToPlay = true;
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
}
