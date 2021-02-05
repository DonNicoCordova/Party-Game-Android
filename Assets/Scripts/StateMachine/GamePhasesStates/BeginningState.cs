
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class Initialize : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public Initialize(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.Instance.initializePhaseTimerDone = true;
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player)
            {
                GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);

        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    public void FixedTick() { }
    public void OnEnter()
    {
        
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            GameManager.Instance.PopulateMinigamesForRound();
        }
        GameManager.Instance.ResetStateOnPlayers();

        GameManager.Instance.StartNextRound();

        if (GameSystem.Instance.initializePhaseTimerDone)
            GameSystem.Instance.initializePhaseTimerDone = false;
        if (GameManager.Instance.GetRound() == 0)
        {
            GameManager.Instance.ShowMessage("¡Bienvenido a tu fiestita!");
            //GameManager.Instance.CreatePlayers();
        }
        else
        {
            GameManager.Instance.ShowMessage($"¡Ronda {GameManager.Instance.GetRound()}!");
        }
        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    
    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.initializePhaseTimerDone)
            GameSystem.Instance.initializePhaseTimerDone = false;
        GameManager.Instance.throwController?.AnimateReadyToPlay();
        GameManager.Instance.throwController?.Initialize();
    }
}