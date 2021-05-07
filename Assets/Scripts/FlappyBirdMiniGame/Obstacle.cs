using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [Header("Configurations")]
    public Collider colliderToHide;

    [SerializeField]
    public MeshRenderer[] meshRenderers;

    [SerializeField]
    public Collider[] meshColliders;
    private void Start()
    {
        if (colliderToHide == null)
        {
            colliderToHide = gameObject.GetComponent<Collider>();
            colliderToHide.enabled = true;
        }

    }
    public void Hide()
    {
        foreach (Collider collider in meshColliders)
        {
            collider.enabled = false;
        }
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = false;
        }
    }
    public void Show()
    {
        foreach (Collider collider in meshColliders)
        {
            collider.enabled = true;
        }
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = true;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HideTrigger"))
        {
            Hide();
        }
    }
}
