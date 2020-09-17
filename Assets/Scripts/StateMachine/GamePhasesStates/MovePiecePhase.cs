using UnityEngine;
using UnityEngine.AI;

internal class MovePiecePhase : IState
{
    private readonly GameObject _gameManager;
    private float defaultStayTime = 5f;
    private float defaultTurnTime = 30f;
    private float stayTime;
    private float turnTime;
    private bool turnDone = false;
    public MovePiecePhase(float minimumTime, float minimumTurnTime)
    {
        defaultTurnTime = minimumTurnTime;
        turnTime = defaultTurnTime;
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        //REDUCING MINIUM TIME OF PHASE
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        if (stayTime == 0f)
        {
            GameSystem.instance.movePiecePhaseDone = true;
        }
        PlayerStats actualPlayer = GameManager.instance.GetActualPlayer();
        if (actualPlayer != null){
            Debug.Log($"ActualPlayer {actualPlayer.id}");

            turnTime -= Time.deltaTime;
            turnTime = Mathf.Clamp(turnTime, 0f, Mathf.Infinity);
            Debug.Log($"TURN TIME LEFT: {turnTime}");
            if (turnTime == 0f)
            {
                turnDone = true;
            }

            if (actualPlayer.isPlayer && !GameManager.instance.joystick.activeSelf)
            {
                Debug.Log("ACTUAL PLAYER IS THE PLAYER | ACTIVATING JOYSTICK");
                GameManager.instance.ShowMessage("Oh! eres tú, no te habia visto.");
                GameManager.instance.EnableJoystick();
                MovesIndicatorController movesController = GameManager.instance.throwText.GetComponentInParent<MovesIndicatorController>();
                movesController.MoveToPlayer();
            }
            if (actualPlayer.PlayerDone() || turnDone)
            {
                if (actualPlayer.isPlayer)
                {
                    GameManager.instance.DisableJoystick();
                    MovesIndicatorController movesController = GameManager.instance.throwText.GetComponentInParent<MovesIndicatorController>();
                    movesController.MoveToScreen();
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
        PlayerStats actualPlayer = GameManager.instance.GetActualPlayer();
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        if (actualPlayer == null)
            GameManager.instance.GetNextPlayer();
        Debug.Log("ENTERED MOVEPIECE");
        GameManager.instance.ShowMessage("¡Hora de moverse!");
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        GameManager.instance.DisableJoystick();
        Debug.Log("EXITED MOVEPIECE");
    }
}