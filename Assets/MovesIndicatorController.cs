using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesIndicatorController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void MoveToPlayer()
    {
        animator.ResetTrigger("Corner");
        animator.SetTrigger("Center");
    }
    public void MoveToScreen()
    {
        animator.ResetTrigger("Center");
        animator.SetTrigger("Corner");
    }
}
