using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

internal class MinigameResultsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    private bool setupComplete = false;
    public MinigameResultsPhase(float minimumTime)
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
                setupComplete = true;
                LevelLoader.Instance.FadeIn();
                GameManager.Instance.RefreshPhaseAnimator();
                foreach (PlayerController player in GameManager.Instance.players.OrderBy(o => o.playerStats.throwOrder).ToList())
                {
                    GameManager.Instance.notActionTakenPlayers.Enqueue(player);
                }
                GameManager.Instance.ResumeGUI();
                GameManager.Instance.ResumeBridges();
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
        if (GameManager.Instance.GetMainPlayer().playerStats.wonLastGame)
        {
            GameManager.Instance.ShowMessage("¡FELICIDADES! te ganaste todas estas cosas.");
        } else
        {
            GameManager.Instance.ShowMessage("¡Que penita! Mejor suerte para la proxima...");
        }
        setupComplete = false;
        GameManager.Instance.ResetStateOnPlayers();
        GameManager.Instance.notActionTakenPlayers.Clear();
        GameManager.Instance.timerBar.SetTimeLeft(0);
        GameManager.Instance.actionTakenPlayers.Clear();
        GameManager.Instance.LoadBridges();
    }
    public void OnExit()
    {
        stayTime = defaultStayTime;
    }
}