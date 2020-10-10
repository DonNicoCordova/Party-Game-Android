﻿using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

internal class MoveResultsPhase : IState
{
    private readonly GameObject _gameManager;
    private float defaultStayTime = 5f;
    private float stayTime;
    public MoveResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.instance.moveResultsPhaseDone = true;
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
        if (GameSystem.instance.moveResultsPhaseDone)
            GameSystem.instance.moveResultsPhaseDone = false;
        Debug.Log("ENTERED MOVERESULTSPHASE");
        GameManager.instance.ShowMessage("Este mensaje da risa... creo");
        GameManager.instance.StartNextRound();
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.moveResultsPhaseDone)
            GameSystem.instance.moveResultsPhaseDone = false;
        Debug.Log("EXITED MOVERESULTSPHASE");
    }
}