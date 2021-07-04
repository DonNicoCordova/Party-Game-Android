
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace PoleMiniGame
{
    internal class InstructionsPhase : IState
    {
        private float defaultStayTime = 5f;
        private float stayTime;
        public InstructionsPhase(float minimumTime)
        {
            defaultStayTime = minimumTime;
            stayTime = defaultStayTime;
        }
        public void Tick()
        {
            if (stayTime <= 0f)
            {

                PlayerController player = GameManager.Instance?.GetMainPlayer();
                if (player.playerStats.id != 0)
                {
                    PoleGameManager.Instance.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                }
            }
            stayTime -= Time.deltaTime;
            stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
            PoleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
        }
        public void FixedTick() { }
        public void OnEnter()
        {
            //reset state done
            if (PhotonNetwork.IsMasterClient)
            {
                PoleGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            }
            PoleGameManager.Instance.ResetStateOnPlayers();
            PoleGameManager.Instance.ShowInstructions();
            PoleGameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
            PoleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
        }

    
        public void OnExit()
        {
            PoleGameManager.Instance.HideInstructions();
            stayTime = defaultStayTime;
            PoleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
        }
    }

}