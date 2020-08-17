using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BoxAccelController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuration")]
    [Range(0f, 5f)] public float lerpTime;
    public float limitAngle = 20f;
    public Transform[] dropPosition;
    public float diceRotationAcceleration = 3.0f;
    public float shakeDetectionThreshold;
    public float minShakeInterval = 1.0f;
    public float shakeForceMultiplier = 50f;
    public int maxShakes = 4;
    [Header("Deadzone")]
    public float xDeadZone = 0.1f;
    public float yDeadZone = 0.1f;
    public float zDeadZone = 0.1f;

    private float sqrShakeDetectionThreshold;
    private float timeSinceLastShake;
    private Boolean dicesReadyToPlay = false;
    private Boolean throwFinished = false;

    private GameManager gameManager;
    private Vector2 prevPoint;
    private Vector2 newPoint;
    private Vector2 screenTravel;
    private int shakesLeft;
    public void OnPointerDown(PointerEventData data)
    {
        if (!IsThrowFinished())
        {
            shakesLeft = maxShakes;
            GameManager.instance.shakesText.text = shakesLeft.ToString();
            prevPoint = data.position;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (!IsThrowFinished())
        {
            newPoint = data.position;
            screenTravel = newPoint - prevPoint;
            _processSwipe(data.delta);
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!IsThrowFinished())
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsThrowFinished())
        {
            if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold && Time.unscaledTime >= timeSinceLastShake + minShakeInterval && shakesLeft > 0)
            {
                ShakeRigidBodies();
                timeSinceLastShake = Time.unscaledTime;
            }
        }
        
    }

    public void ShakeRigidBodies()
    {
        if (!IsThrowFinished())
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
        Debug.Log("INITIALIZING BOX");
        bool finishedMoving = true;
        Debug.Log($"DICES FOUND: {gameManager.dicesInPlay.Length}");
        for (int i = 0; i < gameManager.dicesInPlay.Length; i++)
        {
            Rigidbody rigidbody = gameManager.dicesInPlay[i].GetComponent<Rigidbody>();
            gameManager.MoveDie(rigidbody, dropPosition[i]);
            if (rigidbody.position != dropPosition[i].position)
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
            }
        }



    }
    public float GetShakesLeft() => shakesLeft;

    public bool DicesReadyToPlay() => dicesReadyToPlay;
    
    public void FinishedThrow() => throwFinished = true;
    public bool IsThrowFinished() => throwFinished;

}
