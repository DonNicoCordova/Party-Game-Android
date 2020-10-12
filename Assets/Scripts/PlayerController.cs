using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float _moveSpeed = 10f;

    private CharacterController _characterController;
    [Header("Info")]
    public FloatingJoystick joystick = null;
    public PlayerStats playerStats = null;
    public Rigidbody rig;
    public Player photonPlayer;
    public bool IsGrounded;
    public Animator animator;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("CapturePoint") && playerStats.MovesLeft() != 0)
        {
            Debug.Log($"COLLISION DETECTED {other}");
            LocationController controller = other.GetComponentInParent<LocationController>();
            if (!controller.CheckIfPlayerOnTop(playerStats))
            {
                controller.AddPlayer(playerStats);
                playerStats.CaptureZone(controller);
                if (GameManager.instance.ActualPlayerIsMainPlayer())
                {
                    GameManager.instance.SetThrowText();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CapturePoint"))
        {
            LocationController controller = other.GetComponentInParent<LocationController>();
            controller.RemovePlayer(playerStats);
        }
    }
    void Awake() => _characterController = GetComponent<CharacterController>();
    void Update()
    {
        animator.SetFloat("Horizontal", rig.velocity.x);
        animator.SetFloat("Vertical", rig.velocity.z);
        animator.SetFloat("Speed", rig.velocity.magnitude);
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (joystick.gameObject.activeSelf)
            {
                float vertical = joystick.Vertical;
                float horizontal = joystick.Horizontal;
                Vector3 direction = new Vector3(horizontal, 0, vertical);
                Vector3 movement = transform.TransformDirection(direction) * _moveSpeed;
                IsGrounded = _characterController.SimpleMove(movement);
            }
        }
    }
    [PunRPC]
    public void Initialize(Player newPhotonPlayer)
    {
        // create player stats
        PlayerStats newPlayerStats = new PlayerStats();
        newPlayerStats.id = newPhotonPlayer.ActorNumber;
        newPlayerStats.nickName = newPhotonPlayer.NickName;
        newPlayerStats.mainColor = GameManager.instance.playerConfigs[newPhotonPlayer.ActorNumber-1].mainColor;
        newPlayerStats.orbColor = GameManager.instance.playerConfigs[newPhotonPlayer.ActorNumber-1].orbColor;
        newPlayerStats.SetPlayerGameObject(this.gameObject);

        Debug.Log($"CREATING NEW PLAYER WITH NICKNAME: {newPlayerStats.nickName} MAINCOLOR: {newPlayerStats.mainColor.name} ACTORNUMBER: {newPhotonPlayer.ActorNumber}");
        // change player color
        PlayerGraficsController gfxController = gameObject.GetComponentInChildren<PlayerGraficsController>();
        gfxController.ChangeMaterial(newPlayerStats.mainColor);
        
        // set photon player
        photonPlayer = newPhotonPlayer;

        // add player to player list
        playerStats = newPlayerStats;
        GameManager.instance.players.Add(this);
        
        // Only main player should be affected by physics
        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
        else
        {
            // set player to main player and assign camera to follow plus enable joystick
            GameManager.instance.SetMainPlayer(this);
            GameManager.instance.virtualCamera.Follow = transform;
            GameManager.instance.virtualCamera.LookAt = transform;
            joystick = GameManager.instance.joystick.GetComponent<FloatingJoystick>();
        }
    }

    public void SetStateDone()
    {
        Debug.Log($"Setting state done to {playerStats.id}");
        GameManager.instance.photonView.RPC("SendMessage")
        playerStats.currentStateFinished = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {   
        if (stream.IsWriting)
        {
            string playerStatsObj = JsonUtility.ToJson(playerStats);
            stream.SendNext(playerStatsObj);
        } else if (stream.IsReading)
        {
            string playerStatsObj = (string) stream.ReceiveNext();
            PlayerStats receivedPlayerStats = JsonUtility.FromJson<PlayerStats>(playerStatsObj);
            playerStats.capturedZones = receivedPlayerStats.capturedZones;
            playerStats.ladderPosition = receivedPlayerStats.ladderPosition;
            playerStats.money = receivedPlayerStats.money;
            playerStats.mana = receivedPlayerStats.mana;
            playerStats.moved = receivedPlayerStats.moved;
            playerStats.usedSkill = receivedPlayerStats.usedSkill;
            playerStats.passed = receivedPlayerStats.passed;
            playerStats.currentStateFinished = receivedPlayerStats.currentStateFinished;

        }

    }
}