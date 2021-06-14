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
    private MovesIndicatorController movesIndicator;
    private PlayerController lastPlayer;
    private bool fetchingPlayer = false;
    public MovePiecePhase(float minimumTime, float minimumTurnTime)
    {
        defaultTurnTime = minimumTurnTime;
        turnTime = defaultTurnTime;
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (!fetchingPlayer)
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
                if (turnTime == 0f && turnTimerDone == false)
                {
                    turnTimerDone = true;
                    fetchingPlayer = true;
                }
                if (turnTimerDone && SkillsUI.Instance.noAnimationsPlaying && PhotonNetwork.IsMasterClient)
                {
                    actualPlayer.character.Move(Vector3.zero);
                    GameManager.Instance.GetNextPlayer();
                }
            }
            // IF ACTUAL PLAYER IS IN SYNC AND PLAYER IS DONE PLAYING
            else if (actualPlayer != null && actualPlayer.playerStats.PlayerDone() && lastPlayer == actualPlayer)
            {
                fetchingPlayer = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    actualPlayer.character.Move(Vector3.zero);
                    GameManager.Instance.GetNextPlayer();
                }
            }
        }
    }

    public void FixedTick() {
    }
    public void OnEnter()
    {
        lastPlayer = null;
        GameManager.Instance.ActualPlayerChanged += (sender, args) => OnActualPlayerChange();
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
            GameManager.Instance.GetNextPlayer();
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

        GameManager.Instance.ActualPlayerChanged -= (sender, args) => OnActualPlayerChange();
    }
    private void OnActualPlayerChange()
    {
        turnTime = defaultTurnTime;
        turnTimerDone = false;
        PlayerController mainPlayer = GameManager.Instance.GetMainPlayer();
        if (lastPlayer == mainPlayer)
        {
            GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, lastPlayer.playerStats.id);
            mainPlayer.rig.velocity = Vector3.zero;
            mainPlayer.rig.angularVelocity = Vector3.zero;
        }
        lastPlayer = GameManager.Instance.GetActualPlayer(); 
        
        fetchingPlayer = false;
    }
}