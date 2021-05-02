using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffTree : MonoBehaviour
{
    public Animator animator;
    public float minTime;
    public float maxTime;
    private void Start()
    {
        if (animator == null)
            animator = gameObject.GetComponent<Animator>();
        StartCoroutine(Animate());
    }

    public IEnumerator Animate()
    {
        float waitTime = 0f;
        while (animator != null)
        {
            waitTime = Random.Range(minTime, maxTime);
            animator.SetTrigger("Animate");
            yield return new WaitForSeconds(waitTime);
        }
    }
}

