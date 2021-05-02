using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public Transform destination;

    private SpriteRenderer spriteRenderer;
    private BoxCollider boxCollider;
    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    public void ShowButton()
    {
        spriteRenderer.enabled = true;
    }
    public void HideButton()
    {
        if (gameObject.name == "Bridge (9)")
            Debug.Log("BRIDGE 9 IS HIDING BUTTONS");
        spriteRenderer.enabled = false;
    }
    public void EnableButton()
    {
        boxCollider.enabled = true;
    }
    public void DisableButton()
    {
        boxCollider.enabled = false;
    }
    public void AnimatePress()
    {
        animator.SetTrigger("Press");
    }
}
