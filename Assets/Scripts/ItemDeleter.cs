using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDeleter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTER");
        if (other.CompareTag("FallingItem"))
        {
            Destroy(other.gameObject);
        }
    }
}
