using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

internal class MinigameResultsPhase : IState
{
    private float defaultStayTime = 10f;
    private float stayTime;
    private bool setupComplete = false;
    public MinigameResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
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
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.SetCurrentState(this.GetType().Name);
        }
        if (GameManager.Instance.GetMainPlayer().playerStats.wonLastGame)
        {
            GameManager.Instance.ShowMessage("¡FELICIDADES! te ganaste todas estas cosas.");
            PrizesUI.Instance.GivePrizesToMainPlayer(5);
        } else
        {
            GameManager.Instance.ShowMessage("¡Que penita! Mejor suerte para la proxima...");
        }
    }
    public void OnExit()
    {
        PrizesUI.Instance.Hide();
        GameManager.Instance.notActionTakenPlayers.Clear();
        GameManager.Instance.actionTakenPlayers.Clear();
        stayTime = defaultStayTime;
    }
}