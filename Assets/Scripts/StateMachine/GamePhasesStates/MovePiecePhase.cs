using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

internal class MovePiecePhase : IState
{
    private readonly GameObject _gameManager;
    private float defaultStayTime = 5f;
    private float defaultTurnTime = 30f;
    private float stayTime;
    private float turnTime;
    private bool turnDone = false;
    private MovesIndicatorController movesIndicator;
    public MovePiecePhase(float minimumTime, float minimumTurnTime)
    {
        defaultTurnTime = minimumTurnTime;
        turnTime = defaultTurnTime;
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        if (stayTime == 0f)
        {
            GameSystem.Instance.movePiecePhaseTimerDone = true;
        }
        //Only mainClient can rotate over players and only informs the other clients
        PlayerController actualPlayer = GameManager.Instance.GetActualPlayer();
        if (actualPlayer != null)
        {
            turnTime -= Time.deltaTime;
            turnTime = Mathf.Clamp(turnTime, 0f, Mathf.Infinity);
            GameManager.Instance.timerBar.SetTimeLeft(turnTime);
            if (turnTime == 0f)
            {
                turnDone = true;
            }
            if (GameManager.Instance.ActualPlayerIsMainPlayer() && !GameManager.Instance.joystick.activeSelf)
            {
                GameManager.Instance.ShowMessage("¡Te toca!");
                GameManager.Instance.EnableJoystick();
                SkillsUI.Instance.EnableSkillsButton();
            }
            if (actualPlayer.playerStats.PlayerDone() || turnDone)
            {
                if (GameManager.Instance.ActualPlayerIsMainPlayer())
                {
                    PlayerController player = GameManager.Instance?.GetMainPlayer();
                    if (player)
                    {
                        GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                    }
                    actualPlayer.rig.velocity = Vector3.zero;
                    actualPlayer.rig.angularVelocity = Vector3.zero;
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    GameboardRPCManager.Instance.photonView.RPC("GetNextPlayer", RpcTarget.AllBuffered);
                }
                turnTime = defaultTurnTime;
            }
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("GetNextPlayer", RpcTarget.AllBuffered);
        }
    }

    public void FixedTick() {
    }
    public void OnEnter()
    {
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        GameManager.Instance.ResetStateOnPlayers();
        PlayerController actualPlayer = GameManager.Instance.GetActualPlayer();
        if (GameSystem.Instance.movePiecePhaseTimerDone)
            GameSystem.Instance.movePiecePhaseTimerDone = false;
        if (actualPlayer == null && PhotonNetwork.IsMasterClient)
            GameboardRPCManager.Instance.photonView.RPC("GetNextPlayer", RpcTarget.AllBuffered);
        GameManager.Instance.ShowMessage("¡Hora de moverse!");
        turnTime = defaultTurnTime;
        GameManager.Instance.timerBar.SetMaxTime(defaultTurnTime);
        GameManager.Instance.timerBar.SetTimeLeft(turnTime);
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.Instance.movePiecePhaseTimerDone)
            GameSystem.Instance.movePiecePhaseTimerDone = false;
    }
}