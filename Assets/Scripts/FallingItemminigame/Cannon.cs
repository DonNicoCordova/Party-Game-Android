using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public enum CannonType
    {
        right,
        left
    };
    public CannonType type;
    public Animator animator;
    public float fireForce = 1000f;
    private GameObject[] itemsToThrow;
    private Canvas canvasToSpawn;

    public void Setup(GameObject[] newItemsToThrow, Canvas newCanvasToSpawn)
    {
        canvasToSpawn = newCanvasToSpawn;
        itemsToThrow = newItemsToThrow;
    }
    public void FireLeft()
    {
        foreach (GameObject item in itemsToThrow)
        {
            FallingItemController itemStats = item.GetComponent<FallingItemController>();
            if (!itemStats.fallingItem.isAttackItem)
            {
                GameObject newItem = Instantiate<GameObject>(item, transform.position, Quaternion.identity, transform);
                DragController dragController = newItem.GetComponent<DragController>();
                dragController.SetCanvas(canvasToSpawn);
                Destroy(newItem, 20);
                Rigidbody2D rigidbody = newItem.GetComponent<Rigidbody2D>();
                rigidbody.AddForce(new Vector2(1, 1) * fireForce);
            }
        }
    }
    public void FireRight()
    {
        foreach (GameObject item in itemsToThrow)
        {
            FallingItemController itemStats = item.GetComponent<FallingItemController>();
            if (!itemStats.fallingItem.isAttackItem)
            {
                GameObject newItem = Instantiate<GameObject>(item, transform.position, Quaternion.identity, transform);
                DragController dragController = newItem.GetComponent<DragController>();
                dragController.SetCanvas(canvasToSpawn);
                Destroy(newItem, 20);
                Rigidbody2D rigidbody = newItem.GetComponent<Rigidbody2D>();
                rigidbody.AddForce(new Vector2(-1, 1) * fireForce);
            }
        }
    }
    public void AnimateFire()
    {
        if (type == CannonType.right)
        {
            animator.SetTrigger("FireRight");
        } else if (type == CannonType.left)
        {
            animator.SetTrigger("FireLeft");
        }
    }
}
