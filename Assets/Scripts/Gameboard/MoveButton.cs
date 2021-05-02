using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public Transform destination;
    public SpriteRenderer shadowRenderer;

    private SpriteRenderer spriteRenderer;
    private BoxCollider boxCollider;
    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        shadowRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }
    public void ShowButton()
    {
        spriteRenderer.enabled = true;
        shadowRenderer.enabled = true;
    }
    public void HideButton()
    {
        spriteRenderer.enabled = false;
        shadowRenderer.enabled = false;
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
