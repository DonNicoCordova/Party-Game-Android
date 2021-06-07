using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

internal class ResumePhase : IState
{
    private float defaultStayTime = 10f;
    private float stayTime;
    private bool setupComplete = false;
    public ResumePhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
    {
        if (setupComplete)
        {
            if (stayTime <= 0f)
            {
                PlayerController player = GameManager.Instance?.GetMainPlayer();
                if (player && !player.playerStats.currentStateFinished)
                {
                    GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                }
            }
            stayTime -= Time.deltaTime;
            stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        } else
        {
            GameManager.Instance.ConnectReferences();
            if (GameManager.Instance.allReferencesReady && GameManager.Instance.AllPlayersCharacterSpawned())
            {
                GameManager.Instance.RefreshPhaseAnimator();
                foreach (PlayerController player in GameManager.Instance.players.OrderBy(o => o.playerStats.throwOrder).ToList())
                {
                    GameManager.Instance.notActionTakenPlayers.Enqueue(player);
                }
                GameManager.Instance.ResumeGUI();
                GameManager.Instance.ResumeBridges();
                setupComplete = true;
            }
        }
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.Portrait;
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.SetCurrentState(this.GetType().Name);
        }
        setupComplete = false;
    }
    public void OnExit()
    {
        GameManager.Instance.ResetStateOnPlayers();
        GameManager.Instance.actionTakenPlayers.Clear();
        GameManager.Instance.timerBar.SetTimeLeft(0);
        GameManager.Instance.LoadBridges();
        LevelLoader.Instance.FadeIn();
        stayTime = defaultStayTime;
    }
}