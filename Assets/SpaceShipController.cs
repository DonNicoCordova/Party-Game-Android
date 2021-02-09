using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{
    public float velocity = 1;
    public GameObject propulsorParticle;

    private Rigidbody rb;
    private bool enabledToPlay = false;
    private Animator anim;
    private float vertical;
    private void Start()
    {
        rb = gameObject.GetComponentInChildren<Rigidbody>();
        rb.useGravity = false;
        StartCoroutine(EnableFalling());
        anim = gameObject.GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
    private void Update()
    {
        if (rb.velocity.y > 0)
        {
            vertical = Mathf.Clamp((rb.velocity.y / 2.76f), 0, 1);
        } else if (rb.velocity.y < 0 )
        {
            vertical = Mathf.Clamp((rb.velocity.y / 6.5f), -1, 0);
        }
        anim.SetFloat("Vertical", vertical);
        if (Input.GetMouseButtonDown(0) && enabledToPlay)
        {
            Boost();
            rb.velocity = Vector3.up * velocity;
        }
    }
    void Boost()
    {
        propulsorParticle.GetComponentInChildren<ParticleSystem>().Play();
    }
    void Explode()
    {
        GameObject explosion = Instantiate(FlappyRoyaleGameManager.Instance.explosion, transform.position, Quaternion.identity);
        explosion.GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(GarbageCollect(explosion));
        Destroy(gameObject);
    }
    private IEnumerator GarbageCollect(GameObject explosion)
    {
        yield return new WaitForSeconds(3f);
        Destroy(explosion);
    }
    private IEnumerator EnableFalling()
    {
        yield return new WaitForSeconds(3f);
        rb.useGravity = true;
        enabledToPlay = true;
    }
}
