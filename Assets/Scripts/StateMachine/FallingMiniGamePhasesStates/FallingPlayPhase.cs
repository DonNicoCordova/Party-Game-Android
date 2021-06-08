using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FallingPlayPhase : IState
{
    private bool stateDone;
    public FallingPlayPhase()
    {
        stateDone = false;
    }

    public void Tick()
    {
        //THIS NEEDS TO BE CALLED WHEN THE PLAYER NO LONGER HAVE OBJECTS ON THE POOL
        if (FallingGameManager.Instance.itemSpawner.DoneSpawning && !stateDone)
        {
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player.playerStats.id != 0)
            {
                FallingGameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                stateDone = true;
            }
        }
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        FallingGameManager.Instance.ResetStateOnPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            FallingGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        FallingGameManager.Instance.ShuffleBaskets();
        //reset state done
        FallingGameManager.Instance.itemSpawner.Activate();
    }
    public void OnExit()
    {
    }
}