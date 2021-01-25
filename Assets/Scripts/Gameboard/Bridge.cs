using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Bridge : MonoBehaviourPunCallbacks
{
    public Transform cutPosition1;
    public Transform cutPosition2;
    [SerializeField]
    private Animator animator;
    private Transform originalPosition;
    private bool usable = true;
    private int energyCost = 1;
    private void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        originalPosition = transform;
    }
    [PunRPC]
    public void CutOut()
    {
        Vector3 position1 = cutPosition1.position;
        Vector3 position2 = cutPosition2.position;
        Saw saw = GameManager.Instance.saw.GetComponent<Saw>();
        saw.photonView.RPC("SpawnAt", RpcTarget.AllBuffered, position1.x, position1.y, position1.z);
        animator.SetTrigger("Fall");
        saw.photonView.RPC("Cut", RpcTarget.AllBuffered);
        saw.photonView.RPC("SpawnAt", RpcTarget.AllBuffered, position2.x, position2.y, position1.z);
        saw.photonView.RPC("Cut", RpcTarget.AllBuffered);
        usable = false;
    }

    [PunRPC]
    public void Spawn()
    {
        if (!usable)
            animator.SetTrigger("Spawn");
    }

    [PunRPC]
    public void BecomeSticky()
    {
        energyCost = 3;
    }
}
