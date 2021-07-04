using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace PoleMiniGame
{
    internal class PlayPhase : IState
    {
        private bool stateDone;
        public PlayPhase()
        {
            stateDone = false;
        }

        public void Tick()
        {
            //THIS NEEDS TO BE CALLED WHEN THE PLAYER NO LONGER HAVE OBJECTS ON THE POOL
            if (PoleGameManager.Instance.playerStats.piecesLeft == 0 && !stateDone)
            {
                PlayerController player = GameManager.Instance?.GetMainPlayer();
                if (player.playerStats.id != 0)
                {
                    PoleGameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                    stateDone = true;
                    PoleGameManager.Instance.enabledToPlay = false;
                }
            }
        }
        public void FixedTick() { }
        public void OnEnter()
        {
            PoleGameManager.Instance.ResetStateOnPlayers();
            PoleGameManager.Instance.EnableToPlay();
            if (PhotonNetwork.IsMasterClient)
            {
                PoleGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            }
            //reset state done
        }
        public void OnExit()
        {
        }
    }

}