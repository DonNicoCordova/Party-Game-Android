using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace PoleMiniGame
{
    internal class MinigameOverPhase : IState
    {
        private float defaultStayTime = 5f;
        private float stayTime;
        public MinigameOverPhase(float minimumTime)
        {
            defaultStayTime = minimumTime;
            stayTime = defaultStayTime;
        }

        public void Tick()
        {
            if (stayTime == 0f)
            {
                PlayerController player = GameManager.Instance?.GetMainPlayer();
                if (player.playerStats.id != 0)
                {
                    PoleGameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                    PoleGameManager.Instance?.photonView.RPC("SetMinigameDone", RpcTarget.MasterClient, player.playerStats.id);
                }
                stayTime = -1f;
            } else if (stayTime > 0f)
            {
                stayTime -= Time.deltaTime;
                stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
            }
            PoleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
        }
        public void FixedTick() { }
        public void OnEnter()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PoleGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            }

            PoleGameManager.Instance.ResetStateOnPlayers();
            PoleGameManager.Instance.InitializeGameOver();
            PoleGameManager.Instance.ShowGameResults();
            PoleGameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
            PoleGameManager.Instance.timerBar.SetTimeLeft(stayTime);

        }
        public void OnExit()
        {
            stayTime = defaultStayTime;

            PoleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
        }
    }
}