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
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.SetCurrentState(this.GetType().Name);
        }
        PlayerController mainPlayer = GameManager.Instance.GetMainPlayer();
        List<PlayerController> winners = GameManager.Instance.LastMiniGameWinners;
        if (mainPlayer.playerStats.id == winners[0].playerStats.id)
        {
            GameManager.Instance.ShowMessage("¡FELICIDADES! te ganaste todas estas cosas.");
            PrizesUI.Instance.GivePrizesToMainPlayer(5);
        } else if (mainPlayer.playerStats.id == winners[1].playerStats.id)
        {
            GameManager.Instance.ShowMessage("¡FELICIDADES! te ganaste todas estas cosas.");
            PrizesUI.Instance.GivePrizesToMainPlayer(3);
        }
        else if (mainPlayer.playerStats.id == winners[2].playerStats.id)
        {
            GameManager.Instance.ShowMessage("¡FELICIDADES! te ganaste esta cosa.");
            PrizesUI.Instance.GivePrizesToMainPlayer(1);
        }
        else
        {
            GameManager.Instance.ShowMessage("¡Que penita! Mejor suerte para la próxima...");
        }

        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void OnExit()
    {
        PrizesUI.Instance.Hide();
        GameManager.Instance.notActionTakenPlayers.Clear();
        GameManager.Instance.actionTakenPlayers.Clear();
        stayTime = defaultStayTime;
    }
}