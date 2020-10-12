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
            GameSystem.instance.movePiecePhaseDone = true;
        }
        PlayerController actualPlayer = GameManager.instance.GetActualPlayer();
        if (actualPlayer != null){

            turnTime -= Time.deltaTime;
            turnTime = Mathf.Clamp(turnTime, 0f, Mathf.Infinity);
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
                GameManager.instance?.GetMainPlayer().SetStateDone();
                if (GameManager.instance.ActualPlayerIsMainPlayer())
                {
                    GameManager.instance.DisableJoystick();
                    movesIndicator.MoveToScreen();
                    actualPlayer.rig.velocity = Vector3.zero;
                    actualPlayer.rig.angularVelocity = Vector3.zero;
                }
                GameManager.instance.GetNextPlayer();
                turnTime = defaultTurnTime;
            }
        } else
        {
            GameManager.instance.GetNextPlayer();
        }
    }

    public void FixedTick() {
    }
    public void OnEnter()
    {
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"SENDING STATE TO ALL OTHERS {this.GetType().Name}");
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        GameManager.instance.ResetStateOnPlayers();
        PlayerController actualPlayer = GameManager.instance.GetActualPlayer();
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        if (actualPlayer == null)
            GameManager.instance.GetNextPlayer();
        GameManager.instance.ShowMessage("¡Hora de moverse!");
        movesIndicator = GameManager.instance.throwText.GetComponentInParent<MovesIndicatorController>();
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        GameManager.instance.DisableJoystick();
    }
}