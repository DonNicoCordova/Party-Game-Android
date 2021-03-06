using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public SideStats[] sides;
    private Rigidbody diceRb;
    private Collider diceCollider;
    private Animator animator;
    // Start is called before the first frame update
    private void Start()
    {
        diceRb = gameObject.GetComponent<Rigidbody>();
        diceCollider = gameObject.GetComponent<Collider>();
        animator = gameObject.GetComponent<Animator>();
    }
    public void ShakeDice(float diceRotationAcceleration, float shakeForceMultiplier)
    {
        float dirx = Random.Range(0, 360);
        float diry = Random.Range(0, 360);
        float dirz = Random.Range(0, 360);
        Vector3 torqueDirection = new Vector3(dirx, diry, dirz);
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 forceDirection = new Vector3(randomDirection.x, Mathf.Abs(randomDirection.y), randomDirection.z);
        diceRb.AddTorque(torqueDirection * diceRotationAcceleration, ForceMode.Acceleration);
        diceRb.AddForce(forceDirection * shakeForceMultiplier, ForceMode.Impulse); 
    }

    public void Throw(float diceRotationAcceleration, float shakeForceMultiplier)
    {
        if (!diceRb.useGravity)
            diceRb.useGravity = true;
        if (diceCollider.isTrigger)
            diceCollider.isTrigger = false;
        float dirx = Random.Range(0, 360);
        float diry = Random.Range(0, 360);
        float dirz = Random.Range(0, 360);
        Vector3 torqueDirection = new Vector3(dirx, diry, dirz);
        diceRb.AddTorque(torqueDirection * diceRotationAcceleration, ForceMode.Acceleration);
        diceRb.AddForce(Vector3.up * shakeForceMultiplier, ForceMode.Impulse);
    }

    public Vector3 GetVelocity()
    {
        return diceRb.velocity;
    }

    public float GetSideValue(Collider colliderTriggered)
    {
        foreach(SideStats stats in sides)
        {
            if (colliderTriggered == stats.GetSideCollider())
            {
                return stats.GetValue();
            }
        }
        return -1;
    }

    public SideStats GetSideStats(Collider colliderTriggered)
    {
        foreach (SideStats stats in sides)
        {
            if (colliderTriggered == stats.GetSideCollider())
            {
                return stats;
            }
        }
        return null;
    }
    public void MoveToPlayArea()
    {
        animator.ResetTrigger("MoveToScreen");
        animator.SetTrigger("MoveToPlayArea");
    }
    public void MoveToScreen()
    {
        animator.ResetTrigger("MoveToPlayArea");
        animator.SetTrigger("MoveToScreen");
    }
    public bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void DisableAnimator()
    {
        transform.position = transform.position;
        animator.enabled = false;
    }
    public void EnableAnimator()
    {
        animator.enabled = true;
    }
}
