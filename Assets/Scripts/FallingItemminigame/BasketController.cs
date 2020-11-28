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
                FallingGameManager.instance.AddPoints(2);
                catchedItem.Die();
            } else if (catchedItem.fallingItem.isAttackItem)
            {
                PlayerController playerController = GameManager.instance.players.Find(p => p.playerStats.ladderPosition == 1);
                if (cannonToTrigger == Cannon.left)
                {
                    Debug.Log($"FIRING LEFT CANNON TO player {playerController.playerStats.id} FROM: {GameManager.instance.GetMainPlayer().playerStats.id}");
                    FallingGameManager.instance.photonView.RPC("FireLeftCannon", Photon.Pun.RpcTarget.Others, FallingGameManager.instance.GetMostPoints().playerId);
                } else if (cannonToTrigger == Cannon.right)
                {
                    Debug.Log($"FIRING RIGHT CANNON TO player {playerController.playerStats.id} FROM: {GameManager.instance.GetMainPlayer().playerStats.id}");
                    FallingGameManager.instance.photonView.RPC("FireRightCannon", Photon.Pun.RpcTarget.Others, FallingGameManager.instance.GetMostPoints().playerId);
                }
                catchedItem.Die();
            } 
            else
            {
                FallingGameManager.instance.ReducePoints(1);
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
