using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSelectionEffect : MonoBehaviour
{
    public SpriteRenderer minimapSelectableIconFront;
    public SpriteRenderer minimapSelectableIconBack;
    private Animator _animator;
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        DisableAnimator();
    }

    public void EnableAnimator()
    {
        _animator.enabled = true;
        minimapSelectableIconBack.enabled = true;
        minimapSelectableIconFront.enabled = true;
    }
    public void DisableAnimator()
    {
        _animator.enabled = false;
        minimapSelectableIconBack.enabled = false;
        minimapSelectableIconFront.enabled = false;
    }
}
