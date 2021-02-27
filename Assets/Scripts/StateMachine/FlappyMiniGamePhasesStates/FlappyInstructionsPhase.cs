
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FlappyInstructionsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public FlappyInstructionsPhase(float minimumTime)
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
                FlappyRoyaleGameManager.Instance.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        FlappyRoyaleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            FlappyRoyaleGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        FlappyRoyaleGameManager.Instance.ResetStateOnPlayers();
        FlappyRoyaleGameManager.Instance.ShowInstructions();
        FlappyRoyaleGameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        FlappyRoyaleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    
    public void OnExit()
    {
        FlappyRoyaleGameManager.Instance.HideInstructions();
        stayTime = defaultStayTime;
        FlappyRoyaleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
}