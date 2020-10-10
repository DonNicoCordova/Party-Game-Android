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
            GameSystem.instance.orderingPhaseDone = true;

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void FixedTick()
    {
        GameManager.instance.throwController?.CheckIfDicesStopped();
        if (GameManager.instance.throwController.DicesStopped())
        {
            GameManager.instance?.GetMainPlayer().SetStateDone();
        }
    }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"SENDING STATE TO ALL OTHERS {this.GetType().Name}");
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done

        GameManager.instance.ResetStateOnPlayers();
        if (GameSystem.instance.orderingPhaseDone)
            GameSystem.instance.orderingPhaseDone = false;
        Debug.Log("ENTERING ORDERING");
        GameManager.instance.ShowMessage("Decidamos el orden de juego");
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.orderingPhaseDone)
            GameSystem.instance.orderingPhaseDone = false;
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED ORDERING");
    }
}