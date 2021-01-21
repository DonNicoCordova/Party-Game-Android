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
                FallingGameManager.Instance.photonView.RPC("AddPoints",RpcTarget.AllBuffered,GameManager.Instance.GetMainPlayer().playerStats.id);
                catchedItem.Die();
            } else if (catchedItem.fallingItem.isAttackItem)
            {
                if (cannonToTrigger == Cannon.left)
                {
                    FallingGameManager.Instance.photonView.RPC("FireLeftCannon", RpcTarget.Others, FallingGameManager.Instance.GetMostPoints().playerId);
                } else if (cannonToTrigger == Cannon.right)
                {
                    FallingGameManager.Instance.photonView.RPC("FireRightCannon", RpcTarget.Others, FallingGameManager.Instance.GetMostPoints().playerId);
                }
                catchedItem.Die();
            } 
            else
            {
                FallingGameManager.Instance.photonView.RPC("ReducePoints", RpcTarget.AllBuffered, GameManager.Instance.GetMainPlayer().playerStats.id);
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
