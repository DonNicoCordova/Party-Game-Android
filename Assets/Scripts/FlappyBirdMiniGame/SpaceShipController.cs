using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpaceShipController : MonoBehaviourPunCallbacks
{
    public float velocity = 1;
    public GameObject propulsorParticle;
    public ForceShield forceShield;
    public FlappyRoyaleStats playerStats;
    [SerializeField]
    private SpriteRenderer colorRenderer;
    [SerializeField]
    private MeshCollider shipCollider;
    private MeshRenderer shipRenderer;
    private Rigidbody rb;
    [SerializeField]
    private bool enabledToPlay = false;
    private Animator anim;
    private float vertical;
    private Player photonPlayer;
    private float timeAlive;
    private bool enabledToDie = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        anim = gameObject.GetComponent<Animator>();
        shipCollider = gameObject.GetComponent<MeshCollider>();
        shipRenderer = gameObject.GetComponent<MeshRenderer>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine && enabledToPlay && enabledToDie && (collision.gameObject.CompareTag("Path") || collision.gameObject.CompareTag("Obstacle")))
        {
            this.photonView.RPC("Explode",RpcTarget.All, timeAlive);
        }
    }
    private void Update()
    {
        if (enabledToPlay)
        {
            timeAlive += Time.deltaTime;
            playerStats.timeAlive = timeAlive;
        }
        if (Input.GetMouseButtonDown(0) && enabledToPlay)
        {
            Boost();
            rb.velocity = Vector3.up * velocity;
            Debug.DrawRay(transform.position, Vector3.up * velocity, Color.red, 3f);
        }
    }
    private void FixedUpdate()
    {
        if (rb.velocity.y > 0)
        {
            vertical = Mathf.Clamp((rb.velocity.y / 2.76f), 0, 1);
        }
        else if (rb.velocity.y < 0)
        {
            vertical = Mathf.Clamp((rb.velocity.y / 6.5f), -1, 0);
        }

        anim.SetFloat("Vertical", vertical);

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
    private void Explode(float timeAlive, PhotonMessageInfo info)
    {
        GameObject explosion = Instantiate(FlappyRoyaleGameManager.Instance.explosion, transform.position, Quaternion.identity);
        explosion.GetComponentInChildren<ParticleSystem>().Play();
        gameObject.SetActive(false);
        playerStats.alive = false;
        if (info.Sender.ActorNumber != GameManager.Instance.GetMainPlayer().photonPlayer.ActorNumber)
        {
            FlappyRoyaleStats playerToUpdate = FlappyRoyaleGameManager.Instance.GetStats(info.Sender.ActorNumber);
            playerToUpdate.timeAlive = timeAlive;
        }
    }
    public void EnableFalling()
    {
        rb.useGravity = true;
        enabledToPlay = true;
    }
    public void EnableDying()
    {
        enabledToDie = true;
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
        Collider shieldCollider = forceShield.GetComponent<Collider>();

        // Only main player should be affected by physics
        if (!photonView.IsMine)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
            } else
            {
                rb = gameObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            transform.localScale = transform.localScale * 0.7f;
            shieldCollider.enabled = false;
            shipCollider.enabled = false;
        } else
        { 
            FlappyRoyaleGameManager.Instance.mainPlayer = this;
            shipCollider.isTrigger = false;
            shieldCollider.isTrigger = false;
        }
    }
}
