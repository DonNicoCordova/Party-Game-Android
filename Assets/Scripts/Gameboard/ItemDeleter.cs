using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDeleter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FallingItem") && !other.isTrigger)
        {
            FallingItemController catchedItem = other.GetComponent<FallingItemController>();
            catchedItem.Die();
        }
    }
}
