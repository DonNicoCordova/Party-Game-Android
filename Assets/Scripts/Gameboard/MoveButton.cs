using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public Transform destination;

    private SpriteRenderer spriteRenderer;
    private BoxCollider boxCollider;
    private Animator animator;
    public Transform farEnd;
    private Vector3 frometh;
    private Vector3 untoeth;
    private float secondsForOneLength = 1f;

    void Update()
    {
        transform.position = Vector3.Lerp(frometh, untoeth,
         Mathf.SmoothStep(0f, 1f,
          Mathf.PingPong(Time.time / secondsForOneLength, 1f)
        ));
    }
    private void Start()
    {
        untoeth = transform.position + new Vector3(0,1,0);
        frometh = transform.position + new Vector3(0,2,0);
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
