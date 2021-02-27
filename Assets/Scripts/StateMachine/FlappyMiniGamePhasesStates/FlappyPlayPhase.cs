using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FlappyPlayPhase : IState
{
    private bool stateDone;
    public FlappyPlayPhase()
    {
        stateDone = false;
    }

    public void Tick()
    {
        if (!FlappyRoyaleGameManager.Instance.GetStats(GameManager.Instance.GetMainPlayer().playerStats.id).alive && !stateDone)
        {
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player.playerStats.id != 0)
            {
                FlappyRoyaleGameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                stateDone = true;
            }
        }
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        FlappyRoyaleGameManager.Instance.ResetStateOnPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            FlappyRoyaleGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done
        FlappyRoyaleGameManager.Instance.mainPlayer.EnableFalling();
        PathSpawner.Instance.EnableSpawning();
    }
    public void OnExit()
    {
    }
}