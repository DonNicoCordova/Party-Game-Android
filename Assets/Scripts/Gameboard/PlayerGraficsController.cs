using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraficsController : MonoBehaviour
{
    public SpriteRenderer body;
    public SpriteRenderer rightHand;
    public SpriteRenderer leftHand;
    public SpriteRenderer rightFoot;
    public SpriteRenderer leftFoot;
    public SpriteRenderer minimapIcon;

    public void ChangeMaterial(Material newMaterial)
    {
        body.material = newMaterial;
        rightHand.material = newMaterial;
        leftHand.material = newMaterial;
        rightFoot.material = newMaterial;
        leftFoot.material = newMaterial;
        minimapIcon.material = newMaterial;
    }
}
