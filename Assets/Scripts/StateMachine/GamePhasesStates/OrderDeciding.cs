using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

internal class OrderDecidingPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public OrderDecidingPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        GameManager.Instance.throwController?.CheckInput();

        if (stayTime <= 0f)
        {
            GameSystem.Instance.orderingPhaseTimerDone = true;

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    public void FixedTick()
    {
        GameManager.Instance.throwController?.CheckIfDicesStopped();
        if (GameManager.Instance.throwController.DicesStopped())
        {
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player)
            {
                GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
    }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done

        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.orderingPhaseTimerDone)
            GameSystem.Instance.orderingPhaseTimerDone = false;
        GameManager.Instance.ShowMessage("Decidamos el orden de juego");

        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.orderingPhaseTimerDone)
            GameSystem.Instance.orderingPhaseTimerDone = false;
        GameManager.Instance.throwController?.AnimateFinishedThrow();
    }
}