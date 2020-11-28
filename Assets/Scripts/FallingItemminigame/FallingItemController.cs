using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingItemController : MonoBehaviour
{
    public FallingItem fallingItem;
    public void Die() => Destroy(gameObject);

    //USE THIS CONTROLLER FOR ANIMATION
}

[System.Serializable]
public class FallingItem
{
    public string name;
    public string id;
    public Sprite sprite;
    public bool isAttackItem = false;
}
