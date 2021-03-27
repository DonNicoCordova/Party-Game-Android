using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FlappyPlayPhase : IState
{
    private bool stateDone;
    private float defaultPreparationTime = 3f;
    private float preparationTime;

    public FlappyPlayPhase()
    {
        stateDone = false;
        preparationTime = defaultPreparationTime;
    }
    public void Tick()
    {
        if (preparationTime <= 0f)
        {
            FlappyRoyaleGameManager.Instance.countdownCounter.gameObject.SetActive(false);
            FlappyRoyaleGameManager.Instance.mainPlayer.photonView.RPC("ShieldsDown",RpcTarget.All);
            PathSpawner.Instance.EnableSpawning();
        } else
        {
            Debug.Log($"TRYING TO SET PREPARATION TIME WITH {preparationTime}");
            FlappyRoyaleGameManager.Instance.countdownCounter.text = $"{ preparationTime:0}";
        }
        preparationTime -= Time.deltaTime;
        preparationTime = Mathf.Clamp(preparationTime, 0f, Mathf.Infinity);
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
        FlappyRoyaleGameManager.Instance.countdownCounter.gameObject.SetActive(true);
    }
    public void OnExit()
    {
    }
}