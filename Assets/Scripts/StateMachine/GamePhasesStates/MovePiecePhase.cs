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
    private bool turnTimerDone = false;
    private bool yourTurnMessage = false;
    private MovesIndicatorController movesIndicator;
    private PlayerController lastPlayer;
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
        //Only mainClient can rotate over players and only informs the other clients
        PlayerController actualPlayer = GameManager.Instance.GetActualPlayer();
        //IF ACTUAL PLAYER EXISTS AND IS NOT DONE PLAYING AND IS IN SYNC (LAST PLAYER IS STILL ACTUAL PLAYER)
        if (actualPlayer != null && !actualPlayer.playerStats.PlayerDone() && lastPlayer == actualPlayer)
        {
            if (SkillsUI.Instance.noAnimationsPlaying)
            {
                turnTime -= Time.deltaTime;
                turnTime = Mathf.Clamp(turnTime, 0f, Mathf.Infinity);
            }
            GameManager.Instance.timerBar.SetTimeLeft(turnTime);
            if (turnTime == 0f)
            {
                turnTimerDone = true;
            }
            if (GameManager.Instance.ActualPlayerIsMainPlayer() && !yourTurnMessage)
            {
                GameManager.Instance.ShowMessage("¡Te toca!");
                yourTurnMessage = true;
            }
            if (GameManager.Instance.ActualPlayerIsMainPlayer() && !GameManager.Instance.joystick.activeSelf && SkillsUI.Instance.playerUsingSkills == null)
            {
                GameManager.Instance.EnableJoystick();
            }
            if (turnTimerDone  && SkillsUI.Instance.noAnimationsPlaying)
            {
                GameboardRPCManager.Instance.photonView.RPC("UpdateEnergy", RpcTarget.Others, actualPlayer.photonPlayer.ActorNumber, 0); ;
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

                lastPlayer = actualPlayer;
                if (PhotonNetwork.IsMasterClient)
                {
                    GameboardRPCManager.Instance.photonView.RPC("GetNextPlayer", RpcTarget.All);
                }
                yourTurnMessage = false;
                turnTime = defaultTurnTime;
            }
        }
        // IF NOT IN SYNC. SYNC.
        else if (actualPlayer != null && !actualPlayer.playerStats.PlayerDone() && lastPlayer != actualPlayer)
        {
            lastPlayer = actualPlayer;
            
            yourTurnMessage = false;
            turnTime = defaultTurnTime;
        }
        // IF ACTUAL PLAYER IS IN SYNC AND PLAYER IS DONE PLAYING
        else if (actualPlayer != null && actualPlayer.playerStats.PlayerDone() && lastPlayer == actualPlayer)
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
                GameboardRPCManager.Instance.photonView.RPC("GetNextPlayer", RpcTarget.All);
            }
            yourTurnMessage = false;
            turnTime = defaultTurnTime;
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
        lastPlayer = GameManager.Instance.notActionTakenPlayers.Peek();
        PlayerController actualPlayer = GameManager.Instance.GetActualPlayer();
        if (GameSystem.Instance.movePiecePhaseTimerDone)
            GameSystem.Instance.movePiecePhaseTimerDone = false;
        if (actualPlayer == null && PhotonNetwork.IsMasterClient)
            GameboardRPCManager.Instance.photonView.RPC("GetNextPlayer", RpcTarget.All);
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