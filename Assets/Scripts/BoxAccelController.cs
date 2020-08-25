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

    private GameManager gameManager;
    private Vector2 prevPoint;
    private Vector2 newPoint;
    private Vector2 screenTravel;
    private int shakesLeft;
    private Animator boxAnimator;
    public void OnPointerDown(PointerEventData data)
    {
        if (!IsThrowFinished() && dicesReadyToPlay)
        {
            shakesLeft = maxShakes;
            GameManager.instance.shakesText.text = shakesLeft.ToString();
            prevPoint = data.position;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (!IsThrowFinished() && dicesReadyToPlay)
        {
            newPoint = data.position;
            screenTravel = newPoint - prevPoint;
            _processSwipe(data.delta);
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!IsThrowFinished() && dicesReadyToPlay)
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

    // Start is called before the first frame update
    void Start()
    {
        sqrShakeDetectionThreshold = Mathf.Pow(shakeDetectionThreshold, 2);
        shakesLeft = maxShakes;
        gameManager = GameManager.instance;
        boxAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsThrowFinished() && dicesReadyToPlay)
        {
            if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold && Time.unscaledTime >= timeSinceLastShake + minShakeInterval && shakesLeft > 0)
            {
                ShakeRigidBodies();
                timeSinceLastShake = Time.unscaledTime;
            }
            if (Input.GetMouseButtonDown(0))
            {
                foreach (DiceController die in gameManager.dicesInPlay)
                {
                    die.Throw(diceRotationAcceleration, shakeForceMultiplier);
                }
            }
        }
        
    }

    public void ShakeRigidBodies()
    {
        if (!IsThrowFinished() && dicesReadyToPlay)
        {
            shakesLeft -= 1;
            GameManager.instance.shakesText.text = shakesLeft.ToString();
            for (int i = 0; i < gameManager.dicesInPlay.Length; i++)
            {
                gameManager.dicesInPlay[i].ShakeDice(diceRotationAcceleration, shakeForceMultiplier * 2);
            }
        }
    }

    public void Initialize()
    {
        bool finishedMoving = true;
        for (int i = 0; i < gameManager.dicesInPlay.Length; i++)
        {
            Rigidbody rigidbody = gameManager.dicesInPlay[i].GetComponent<Rigidbody>();
            gameManager.MoveDie(rigidbody, dropPosition[i]);
            if (Vector3.Distance(dropPosition[i].position, rigidbody.position) > 0.2f)
            {
                finishedMoving = false;
            }
        }

        if (finishedMoving != dicesReadyToPlay)
        {
            dicesReadyToPlay = finishedMoving;
            foreach (DiceController controller in gameManager.dicesInPlay)
            {
                Rigidbody rigidbody = controller.GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                float dirx = UnityEngine.Random.Range(0, 360);
                float diry = UnityEngine.Random.Range(0, 360);
                float dirz = UnityEngine.Random.Range(0, 360);
                Vector3 torqueDirection = new Vector3(dirx, diry, dirz);
                rigidbody.AddTorque(torqueDirection * diceRotationAcceleration, ForceMode.Acceleration);
            }
        }



    }
    public float GetShakesLeft() => shakesLeft;

    public bool DicesReadyToPlay() => dicesReadyToPlay;
    
    public void FinishedThrow() => throwFinished = true;
    public bool IsThrowFinished() => throwFinished;

    public void AnimateReadyToPlay() {
        boxAnimator.ResetTrigger("ThrowFinished");
        boxAnimator.SetTrigger("ReadyToPlay");
    }
    public void AnimateFinishedThrow()
    {
        boxAnimator.ResetTrigger("ReadyToPlay");
        boxAnimator.SetTrigger("ThrowFinished");
    }
}
