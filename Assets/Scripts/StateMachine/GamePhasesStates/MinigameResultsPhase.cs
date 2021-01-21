﻿using UnityEngine;
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
                if (player)
                {
                    GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                }
            }
            stayTime -= Time.deltaTime;
            stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
            GameManager.Instance.timerBar?.SetTimeLeft(stayTime);
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
                GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
                GameManager.Instance.timerBar.SetTimeLeft(stayTime);
                GameManager.Instance.ResumeGUI();
            }
        }
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.SetCurrentState(this.GetType().Name);
        }
        GameManager.Instance.ShowMessage("FELICIDADES! te ganaste todas estas cosas.");

        GameManager.Instance.ResetStateOnPlayers();
        GameManager.Instance.notActionTakenPlayers.Clear();
        GameManager.Instance.actionTakenPlayers.Clear();
        GameManager.Instance.timerBar?.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar?.SetTimeLeft(stayTime);

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
}