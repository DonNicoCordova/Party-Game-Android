using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BasketController : MonoBehaviour
{
    public Collider2D detectionCollider;
    public FallingItem itemToCatch;
    public SpriteRenderer itemSpriteRenderer;
    public enum Cannon
    {
        right,
        left
    };
    public Cannon cannonToTrigger = Cannon.left;

    private void Awake()
    {
        UpdateSprite();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FallingItem") && !collision.isTrigger)
        {
            FallingItemController catchedItem = collision.GetComponent<FallingItemController>();
            if (CheckIfItemIsCorrect(catchedItem))
            {
                FallingGameManager.Instance.AddPoints(2);
                catchedItem.Die();
            } else if (catchedItem.fallingItem.isAttackItem)
            {
                if (cannonToTrigger == Cannon.left)
                {
                    Debug.Log("CALLING RPC");
                    FallingGameManager.Instance.photonView.RPC("FireLeftCannon", RpcTarget.Others, FallingGameManager.Instance.GetMostPoints().playerId);
                } else if (cannonToTrigger == Cannon.right)
                {
                    Debug.Log("CALLING RPC");
                    FallingGameManager.Instance.photonView.RPC("FireRightCannon", RpcTarget.Others, FallingGameManager.Instance.GetMostPoints().playerId);
                }
                catchedItem.Die();
            } 
            else
            {
                FallingGameManager.Instance.ReducePoints(1);
                catchedItem.Die();
            }
        }
    }
    public void UpdateSprite()
    {
        itemSpriteRenderer.sprite = itemToCatch.sprite;
    }
    private bool CheckIfItemIsCorrect(FallingItemController catchedItem) => catchedItem.fallingItem.id == itemToCatch.id;

}
