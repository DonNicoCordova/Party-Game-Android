using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;

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
    private Queue<Command> _commands = new Queue<Command>();
    private Command _currentCommand;
    [Header("UI")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI energyText;
    [SerializeField]
    private GameObject energyContainer;
    public bool enabledToPlay = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("CapturePoint") && playerStats.EnergyLeft() > 0)
        {
            LocationController controller = other.GetComponentInParent<LocationController>();
            if (!controller.CheckIfPlayerOnTop(this))
            {
                controller.AddPlayer(this);
                playerStats.CaptureZone(controller);
            }
        }
        else if (other.CompareTag("FallTrigger"))
        {
            ResetPosition();
        }

    }
    public void ProcessCommands()
    {
        if (_currentCommand != null && _currentCommand.IsFinished == false)
            return;

        if (_commands.Any() == false)
            return;

        _currentCommand = _commands.Dequeue();
        _currentCommand.Execute();

        if (_currentCommand.Failed)
        {
            _currentCommand.Reset();
            _commands.Enqueue(_currentCommand);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CapturePoint"))
        {
            LocationController controller = other.GetComponentInParent<LocationController>();
            controller.RemovePlayer(this);
        }
    }
    private void Awake() => _characterController = GetComponent<CharacterController>();
    private void FixedUpdate()
    {
        if (photonView.IsMine && enabledToPlay)
        {
            if (joystick.gameObject.activeSelf && GameManager.Instance.ActualPlayerIsMainPlayer())
            {
                float vertical = joystick.Vertical;
                float horizontal = joystick.Horizontal;
                Vector3 direction = new Vector3(horizontal, 0, vertical);
                Vector3 movement = transform.TransformDirection(direction) * _moveSpeed;
                IsGrounded = _characterController.SimpleMove(movement);
                animator.SetFloat("Horizontal", horizontal);
                animator.SetFloat("Vertical", vertical);
                animator.SetFloat("Speed", rig.velocity.magnitude);
            }
            else
            {
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 0);
                animator.SetFloat("Speed", 0);
            }
        }
    }
    private void Update()
    {
        ProcessCommands();

    }
    [PunRPC]
    public void Initialize(Player newPhotonPlayer)
    {
        // create player stats
        PlayerStats newPlayerStats = new PlayerStats();
        newPlayerStats.id = newPhotonPlayer.ActorNumber;
        newPlayerStats.nickName = newPhotonPlayer.NickName;
        newPlayerStats.mainColor = GameManager.Instance.playerConfigs[newPhotonPlayer.ActorNumber - 1].mainColor;
        newPlayerStats.orbColor = GameManager.Instance.playerConfigs[newPhotonPlayer.ActorNumber - 1].orbColor;
        newPlayerStats.SetPlayerGameObject(this.gameObject);
        // change player color
        PlayerGraficsController gfxController = gameObject.GetComponentInChildren<PlayerGraficsController>();
        gfxController.ChangeMaterial(newPlayerStats.mainColor);
        energyText.text = "0";
        playerNameText.text = newPhotonPlayer.NickName;
        // set photon player
        photonPlayer = newPhotonPlayer;

        // add player to player list
        playerStats = newPlayerStats;
        GameManager.Instance.players.Add(this);

        // Only main player should be affected by physics
        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
        else
        {
            // set player to main player and assign camera to follow plus enable joystick
            GameManager.Instance.SetMainPlayer(this);
            GameManager.Instance.virtualCamera.Follow = transform;
            GameManager.Instance.virtualCamera.LookAt = transform;
            joystick = GameManager.Instance.joystick.GetComponent<FloatingJoystick>();
        }
        playerStats.EnergyChanged += (sender, args) => UpdateEnergy();
        HideEnergyContainer();
        enabledToPlay = true;
    }
    [PunRPC]
    public void Resume(Player newPhotonPlayer)
    {
        GameManager.Instance.players.Add(this);
        GameManager.Instance.LoadPlayers();
        ResumeCommand resumeCommand = new ResumeCommand(newPhotonPlayer, this);
        _commands.Enqueue(resumeCommand);
    }
    [PunRPC]
    public void SetPlayerPlace(int position)
    {
        playerStats.ladderPosition = position;
    }
    public void ResetPosition()
    {
        gameObject.SetActive(false);
        transform.position = playerStats.lastSpawnPosition.position;
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        playerStats.AddEnergy(1);
        gameObject.SetActive(true);
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
            playerStats.ladderPosition = receivedPlayerStats.ladderPosition;
            playerStats.wonLastGame = receivedPlayerStats.wonLastGame;
            playerStats.SetEnergyLeft(receivedPlayerStats.EnergyLeft());
            energyText.text = receivedPlayerStats.EnergyLeft().ToString();
            playerStats.passed = receivedPlayerStats.passed;
        }

    }
    public void UpdateEnergy()
    {
        energyText.text = playerStats.EnergyLeft().ToString();
        if (photonView.IsMine && GameManager.Instance.energyCounter != null)
        {
            GameManager.Instance.energyCounter.SetEnergy(playerStats.EnergyLeft());
        }
    }
    public void WonMiniGame()
    {
        playerStats.wonLastGame = true;
    }
    public void ResetForNewRound()
    {
        playerStats.passed = false;
        playerStats.currentStateFinished = false;
        playerStats.currentMinigameStateFinished = false;
    }

    private void OnDestroy()
    {
        playerStats.EnergyChanged -= (sender, args) => UpdateEnergy();
    }
    public void ShowEnergyContainer() 
    {
        if (!energyContainer.activeSelf)
        {
            energyContainer.SetActive(true);
        }
    }
    private void HideEnergyContainer() 
    {
        energyContainer.SetActive(false);
    }
}