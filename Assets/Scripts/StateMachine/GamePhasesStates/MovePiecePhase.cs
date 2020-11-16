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
            GameSystem.instance.movePiecePhaseTimerDone = true;
        }
        //Only mainClient can rotate over players and only informs the other clients
        PlayerController actualPlayer = GameManager.instance.GetActualPlayer();
        if (actualPlayer != null)
        {
            turnTime -= Time.deltaTime;
            turnTime = Mathf.Clamp(turnTime, 0f, Mathf.Infinity);
            GameManager.instance.timerBar.SetTimeLeft(turnTime);
            if (turnTime == 0f)
            {
                turnDone = true;
            }

            if (GameManager.instance.ActualPlayerIsMainPlayer() && !GameManager.instance.joystick.activeSelf)
            {
                GameManager.instance.ShowMessage("Oh! eres tú, no te habia visto.");
                GameManager.instance.EnableJoystick();
                movesIndicator.MoveToPlayer();
            }
            if (actualPlayer.playerStats.PlayerDone() || turnDone)
            {
                if (GameManager.instance.ActualPlayerIsMainPlayer())
                {
                    PlayerController player = GameManager.instance?.GetMainPlayer();
                    if (player)
                    {
                        GameManager.instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                    }
                    movesIndicator.MoveToScreen();
                    actualPlayer.rig.velocity = Vector3.zero;
                    actualPlayer.rig.angularVelocity = Vector3.zero;
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    GameManager.instance.photonView.RPC("GetNextPlayer", RpcTarget.AllBuffered);
                }
                turnTime = defaultTurnTime;
            }
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("GetNextPlayer", RpcTarget.AllBuffered);
        }
    }

    public void FixedTick() {
    }
    public void OnEnter()
    {
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        GameManager.instance.ResetStateOnPlayers();
        PlayerController actualPlayer = GameManager.instance.GetActualPlayer();
        if (GameSystem.instance.movePiecePhaseTimerDone)
            GameSystem.instance.movePiecePhaseTimerDone = false;
        if (actualPlayer == null && PhotonNetwork.IsMasterClient)
                GameManager.instance.photonView.RPC("GetNextPlayer", RpcTarget.AllBuffered);
        GameManager.instance.ShowMessage("¡Hora de moverse!");
        movesIndicator = GameManager.instance.throwText.GetComponentInParent<MovesIndicatorController>();
        turnTime = defaultTurnTime;
        GameManager.instance.timerBar.SetMaxTime(defaultTurnTime);
        GameManager.instance.timerBar.SetTimeLeft(turnTime);
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.movePiecePhaseTimerDone)
            GameSystem.instance.movePiecePhaseTimerDone = false;
    }
}