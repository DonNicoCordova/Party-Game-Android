using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    [Header("Light Colors")]
    public Color closingColor;
    public Color defaultColor;

    [Header("Configurations")]
    public Light[] lights;
    public Collider colliderToClose;
    public Animator anim;

    private void Start()
    {
        if (anim == null)
        {
            anim = gameObject.GetComponentInChildren<Animator>();
        }
        if (lights == null)
        {
            lights = gameObject.GetComponentsInChildren<Light>();
        }
        if (colliderToClose == null)
        {
            colliderToClose = gameObject.GetComponent<Collider>();
        }
    }

    public void EnableToClose()
    {
        foreach (Light light in lights)
        {
            light.color = closingColor;
        }
        colliderToClose.enabled = true;
    }

    public void DisableToClose()
    {
        foreach (Light light in lights)
        {
            light.color = defaultColor;
        }
        colliderToClose.enabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CloseDoorTrigger"))
        {
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CloseLeft")
            {
                CloseRight();
            } else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CloseRight")
            {
                CloseLeft();
            }
        }
    }
    public void CloseLeft()
    {
        anim.ResetTrigger("closeRight");
        anim.SetTrigger("closeLeft");
    }
    public void CloseRight()
    {
        anim.ResetTrigger("closeLeft");
        anim.SetTrigger("closeRight");
    }
}
