using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTER");
        PathSpawner.Instance.MovePath(gameObject);
    }
}
