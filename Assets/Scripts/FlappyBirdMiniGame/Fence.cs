using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    private BoxCollider boxCollider;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = gameObject.GetComponentInChildren<BoxCollider>();
        anim = gameObject.GetComponent<Animator>();
    }

    public void Remove()
    {
        boxCollider.isTrigger = true;
        anim.SetTrigger("Explode");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
