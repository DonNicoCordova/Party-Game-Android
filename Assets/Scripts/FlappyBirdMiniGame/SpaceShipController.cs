using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpaceShipController : MonoBehaviourPunCallbacks
{
    public float velocity = 1;
    public GameObject propulsorParticle;

    [SerializeField]
    private SpriteRenderer colorRenderer;
    [SerializeField]
    private MeshCollider shipCollider;
    private Rigidbody rb;
    [SerializeField]
    private bool enabledToPlay = false;
    private Animator anim;
    private float vertical;
    private Player photonPlayer;
    private FlappyRoyaleStats playerStats;
    private float timeAlive;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        anim = gameObject.GetComponent<Animator>();
        shipCollider = gameObject.GetComponent<MeshCollider>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine && enabledToPlay)
            this.photonView.RPC("Explode",RpcTarget.All);
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
        if (enabledToPlay)
        {
            timeAlive += Time.deltaTime;
            playerStats.timeAlive = timeAlive;
        }
    }
    public void SetColor(Material newMaterial)
    {
        colorRenderer.color = newMaterial.color;
    }
    private void Boost()
    {
        propulsorParticle.GetComponentInChildren<ParticleSystem>().Play();
    }
    [PunRPC]
    private void Explode()
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
    public void EnableFalling()
    {
        rb.useGravity = true;
        enabledToPlay = true;
    }
    [PunRPC]
    public void Initialize(Player newPhotonPlayer)
    {
        // change player color
        Material newColor = GameManager.Instance.playerConfigs[newPhotonPlayer.ActorNumber - 1].mainColor;
        SetColor(newColor);
        // set photon player
        playerStats = FlappyRoyaleGameManager.Instance.GetStats(newPhotonPlayer.ActorNumber);
        photonPlayer = newPhotonPlayer;

        // Only main player should be affected by physics
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        } else
        { 
            FlappyRoyaleGameManager.Instance.mainPlayer = this;
            shipCollider.isTrigger = false;
        }
    }
}
