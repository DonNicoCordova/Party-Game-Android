
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class WelcomePhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public WelcomePhase(float minimumTime)
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
            if (player && !player.playerStats.currentStateFinished)
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
        GameManager.Instance.InitializeGUI();
        LevelLoader.Instance.FadeIn();
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        GameManager.Instance.ResetStateOnPlayers();

        GameManager.Instance.StartNextRound();

        if (GameSystem.Instance.initializePhaseTimerDone)
            GameSystem.Instance.initializePhaseTimerDone = false;
        GameManager.Instance.ShowMessage("¡Bienvenido a tu fiestita!");
        GameManager.Instance.timerBar.SetTimeLeft(0);
    }

    
    public void OnExit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.PopulateMinigamesForRound();
        }
        stayTime = defaultStayTime;
        if (GameSystem.Instance.initializePhaseTimerDone)
            GameSystem.Instance.initializePhaseTimerDone = false;
        GameManager.Instance.throwController?.Initialize();
        GameManager.Instance.playersLadder.InitializeFromPlayers();
    }
}