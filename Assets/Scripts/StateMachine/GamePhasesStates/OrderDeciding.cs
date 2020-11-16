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
        GameManager.instance.throwController?.CheckInput();

        if (stayTime <= 0f)
        {
            GameSystem.instance.orderingPhaseTimerDone = true;

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    public void FixedTick()
    {
        GameManager.instance.throwController?.CheckIfDicesStopped();
        if (GameManager.instance.throwController.DicesStopped())
        {
            PlayerController player = GameManager.instance?.GetMainPlayer();
            if (player)
            {
                GameManager.instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
    }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done

        GameManager.instance.ResetStateOnPlayers();
        if (GameSystem.instance.orderingPhaseTimerDone)
            GameSystem.instance.orderingPhaseTimerDone = false;
        Debug.Log("ENTERING ORDERING");
        GameManager.instance.ShowMessage("Decidamos el orden de juego");

        GameManager.instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.instance.orderingPhaseTimerDone)
            GameSystem.instance.orderingPhaseTimerDone = false;
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED ORDERING");
    }
}