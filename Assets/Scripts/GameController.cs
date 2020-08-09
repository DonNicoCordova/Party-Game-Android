using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject dice;
    public Transform dropPosition;
    public GameObject box;
    public float stabilizeAfterSeconds = 3.0f;

    [Header("Force Values")]
    public float force = 100f;
    public Vector3 rotationDirection = new Vector3(0, 0, 1);

    [Header("Deadzone")]
    public float xDeadZone = 0.3f;
    public float yDeadZone = 0.01f;
    public float zDeadZone = 0.3f;

    private Vector3 lastStableAcceleration;
    private float rebalanceCountdown;
    private void Start()
    {
        lastStableAcceleration = Input.acceleration;
        rebalanceCountdown = stabilizeAfterSeconds;
    }
    void Update()
    {
        Vector3 dir = new Vector3(0, 0, 0);
        Vector3 deviceRotation = Input.acceleration;
        Vector3 tilt = deviceRotation - lastStableAcceleration;
        lastStableAcceleration = deviceRotation;
        Boolean tiltDetected = false;

        
        if (tilt.y > yDeadZone || tilt.y < -yDeadZone)
        {
            if (!tiltDetected)
                tiltDetected = true;
            dir.z = -tilt.y;
            
        } 
        
        if (tilt.z > zDeadZone || tilt.z < -zDeadZone)
        {
            if (!tiltDetected)
                tiltDetected = true;
            dir.x = -tilt.z;
            
        }
        if (Input.touchCount != 0)
        {
            dice.transform.position = dropPosition.position;            
        }

       
        if (tiltDetected)
        {
            rebalanceCountdown = stabilizeAfterSeconds;
            Rigidbody rigidbody = box.GetComponent<Rigidbody>();
            rigidbody.AddTorque(dir * force);
        }
        //else
        //{
        //    rebalanceCountdown -= Time.deltaTime;
        //    if (rebalanceCountdown < 0 && box.transform.eulerAngles != Vector3.zero)
        //    {
        //        Debug.Log("REBALANCING");
        //        box.transform.rotation = Quaternion.identity;
        //        rebalanceCountdown = 0;
        //    }
        //}

    }

}
