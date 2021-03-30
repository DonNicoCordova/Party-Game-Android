
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class InitializeRound : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public InitializeRound(float minimumTime)
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

    }

    public void FixedTick() { }
    public void OnEnter()
    {
        
        GameManager.Instance.timerBar.SetTimeLeft(0);
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        GameManager.Instance.ResetStateOnPlayers();

        GameManager.Instance.StartNextRound();

        if (GameSystem.Instance.initializePhaseTimerDone)
            GameSystem.Instance.initializePhaseTimerDone = false;

        foreach (PlayerController player in GameManager.Instance.players) {
            player.playerStats.SetEnergyLeft(0);
            player.ShowEnergyContainer(); 
        }
        GameManager.Instance.ShowMessage($"¡Ronda {GameManager.Instance.GetRound()}!");
    }

    
    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.Instance.initializePhaseTimerDone)
            GameSystem.Instance.initializePhaseTimerDone = false;
        GameManager.Instance.throwController?.AnimateReadyToPlay();
        GameManager.Instance.throwController?.Initialize();
    }
}