
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class InstructionsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public InstructionsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {

            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player.playerStats.id != 0)
            {
                FallingGameManager.Instance.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        FallingGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            FallingGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        FallingGameManager.Instance.ResetStateOnPlayers();
        FallingGameManager.Instance.ShowInstructions();
        FallingGameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        FallingGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    
    public void OnExit()
    {
        FallingGameManager.Instance.HideInstructions();
        stayTime = defaultStayTime;
        FallingGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
}