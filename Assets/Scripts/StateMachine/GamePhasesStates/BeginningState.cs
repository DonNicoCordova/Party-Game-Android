
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
            GameSystem.instance.initializePhaseTimerDone = true;
            PlayerController player = GameManager.instance?.GetMainPlayer();
            if (player)
            {
                GameManager.instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);

        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    public void FixedTick() { }
    public void OnEnter()
    {
        
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        GameManager.instance.ResetStateOnPlayers();
        Debug.Log($"ENTERING STATE {GameManager.instance}");

        GameManager.instance.StartNextRound();

        GameManager.instance.throwText.text = "0";

        if (GameSystem.instance.initializePhaseTimerDone)
            GameSystem.instance.initializePhaseTimerDone = false;
        if (GameManager.instance.GetRound() == 0)
        {
            GameManager.instance.ShowMessage("¡Bienvenido a tu fiestita!");
            //GameManager.instance.CreatePlayers();
        }
        else
        {
            GameManager.instance.ShowMessage($"¡Ronda {GameManager.instance.GetRound()}!");
        }
        GameManager.instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    
    public void OnExit()
    {
        Debug.Log("CALLING EXIT OF BEGINNING STATE");
        stayTime = defaultStayTime;
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.instance.initializePhaseTimerDone)
            GameSystem.instance.initializePhaseTimerDone = false;
        GameManager.instance.throwController?.AnimateReadyToPlay();
        GameManager.instance.throwController?.Initialize();
    }
}