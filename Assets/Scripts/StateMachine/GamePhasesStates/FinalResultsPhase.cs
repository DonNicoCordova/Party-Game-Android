using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FinalResultsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public FinalResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.instance.finalResultsPhaseDone = true;
            GameManager.instance?.GetMainPlayer().SetStateDone();
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"SENDING STATE TO ALL OTHERS {this.GetType().Name}");
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done

        GameManager.instance.ResetStateOnPlayers();
        if (GameSystem.instance.finalResultsPhaseDone)
            GameSystem.instance.finalResultsPhaseDone = false;
        Debug.Log("ENTERING FINAL RESULTS");
        GameManager.instance.ShowMessage("Final de la ronda! WOOOOOOO");

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.finalResultsPhaseDone)
            GameSystem.instance.finalResultsPhaseDone = false;
        Debug.Log("EXITING FINAL RESULTS");
    }
}