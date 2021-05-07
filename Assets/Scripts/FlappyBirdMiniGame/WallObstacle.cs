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

    [SerializeField]
    public MeshRenderer[] meshRenderers;

    [SerializeField]
    public MeshCollider[] meshColliders;
    private bool enabledToClose;
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
            colliderToClose.enabled = true;
        }

    }
    public void Hide()
    {
        foreach (MeshCollider mesh in meshColliders)
        {
            mesh.enabled = false;
        }
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = false;
        }
    }
    public void Show()
    {
        foreach (MeshCollider mesh in meshColliders)
        {
            mesh.enabled = true;
        }
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = true;
        }
    }
    public void EnableToClose()
    {
        foreach (Light light in lights)
        {
            light.color = closingColor;
        }
        enabledToClose = true;
    }

    public void DisableToClose()
    {
        foreach (Light light in lights)
        {
            light.color = defaultColor;
        }
        enabledToClose = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (FlappyRoyaleGameManager.Instance.mainPlayer.playerStats.alive)
        {
            if (other.CompareTag("CloseDoorTrigger") && enabledToClose)
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
        if (other.CompareTag("HideTrigger"))
        {
            Hide();
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
