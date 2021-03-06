using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Info")]
    public PlayerStats playerStats = null;
    public Inventory inventory;
    public Rigidbody rig;
    public Player photonPlayer;
    public bool IsGrounded;
    public bool enabledToMove = false;
    public Animator animator;
    public ThirdPersonCharacter character;
    public NavMeshAgent agent;
    public ButtonChecker buttonChecker;

    [Header("UI")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI energyText;


    [SerializeField]
    private GameObject energyContainer;
    private Queue<Command> _commands = new Queue<Command>();
    private Command _currentCommand;
    private bool doneMoving;

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
    private void Start()
    {
        inventory = gameObject.GetComponent<Inventory>();
        agent.updateRotation = false;
        buttonChecker = gameObject.GetComponentInChildren<ButtonChecker>();
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

    private void LateUpdate()
    {
        ProcessCommands();
        if (GameManager.Instance.ActualPlayerIsMainPlayer() && photonView.IsMine && enabledToMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (SkillsUI.Instance.playerUsingSkills != null)
                    return;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, 3f);
                if (Physics.Raycast(ray, out hit, 50f, 1 << LayerMask.NameToLayer("UI")))
                {
                    if (hit.collider.gameObject.CompareTag("MoveButton"))
                    {
                        MoveButton moveButton = hit.collider.gameObject.GetComponent<MoveButton>();
                        moveButton.AnimatePress();
                        enabledToMove = false;
                        photonView.RPC("MoveToTarget", RpcTarget.All, moveButton.destination.position);
                        buttonChecker.HideButtons();
                    }
                }
            }
        }
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
            string playerStatsObj = (string)stream.ReceiveNext();
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
    public void RunCheckingCoRoutine()
    {
        StartCoroutine(CheckIfDoneMoving());
    }
    private IEnumerator CheckIfDoneMoving()
    {
        while (GameManager.Instance.ActualPlayerIsMainPlayer())
        {
            if (!agent.pathPending)
            {
                //Debug.Log($"CHECKING IF agent.remainingDistance <= agent.stoppingDistance => {agent.remainingDistance} <= {agent.stoppingDistance}");
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    //Debug.Log($"CHECKING IF !agent.hasPath || (agent.velocity.sqrMagnitude <= 0.2f && agent.velocity.sqrMagnitude >= -0.2f) => {!agent.hasPath} || ({agent.velocity.sqrMagnitude} <= {0.2f} && {agent.velocity.sqrMagnitude} >= -0.2f)");
                    if (!agent.hasPath || (agent.velocity.sqrMagnitude <= 0.2f && agent.velocity.sqrMagnitude >= -0.2f))
                    {
                        buttonChecker.CheckForButtonsNearby();
                        enabledToMove = true;
                        agent.isStopped = true;
                        agent.ResetPath();
                        character.Move(new Vector3(0f, 0f, 0f));
                        rig.velocity = new Vector3(0f, 0f, 0f);
                        buttonChecker.ShowButtons();
                    }
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    //RPC SECTION
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
        }
        playerStats.EnergyChanged += (sender, args) => UpdateEnergy();
        HideEnergyContainer();
    }
    [PunRPC]
    public void Resume(Player newPhotonPlayer)
    {
        GameManager.Instance.players.Add(this);
        ResumeCommand resumeCommand = new ResumeCommand(newPhotonPlayer, this);
        _commands.Enqueue(resumeCommand);
    }
    [PunRPC]
    public void SetPlayerPlace(int position)
    {
        playerStats.ladderPosition = position;
    }
    [PunRPC]
    public void StartThinking()
    {
        animator.SetBool("Thinking", true);
    }
    [PunRPC]
    public void StopThinking()
    {
        animator.SetBool("Thinking", false);
    }
    [PunRPC]
    public void MoveToTarget(Vector3 destination)
    {
        agent.SetDestination(destination);
        character.Move(destination - transform.position);
    }
}