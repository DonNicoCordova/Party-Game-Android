using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FlappyPlayPhase : IState
{
    private bool stateDone;
    private float defaultPreparationTime = 5f;
    private float defaultFirstDifficultyTime = 10f;
    private float defaultSecondDifficultyTime = 15f;
    private float difficultyUpgradeTime;
    private float preparationTime;
    private FlappyRoyaleGameManager gameManager;
    public FlappyPlayPhase()
    {
        stateDone = false;
        difficultyUpgradeTime = defaultFirstDifficultyTime;
        preparationTime = defaultPreparationTime;
        gameManager = FlappyRoyaleGameManager.Instance;
    }
    public void Tick()
    {
        if (preparationTime <= 0f && gameManager.actualDifficulty.difficulty == Difficulty.Passive)
        {
            FlappyRoyaleGameManager.Instance.countdownCounter.gameObject.SetActive(false);
            gameManager.DifficultyUp();
            PathSpawner.Instance.EnableSpawning();
            FlappyRoyaleGameManager.Instance.mainPlayer.EnableDying();
        } else
        {
            FlappyRoyaleGameManager.Instance.countdownCounter.text = $"{ preparationTime:0}";
        }

        if (difficultyUpgradeTime <= 0f)
        {
            gameManager.DifficultyUp();
            difficultyUpgradeTime = defaultSecondDifficultyTime;
            if (gameManager.actualDifficulty.difficulty == Difficulty.Easy)
            {
                FlappyRoyaleGameManager.Instance.RemoveBottomFences();
            } else if (gameManager.actualDifficulty.difficulty == Difficulty.Medium)
            {
                FlappyRoyaleGameManager.Instance.RemoveTopFences();
            }

            
        }

        if (!FlappyRoyaleGameManager.Instance.GetStats(GameManager.Instance.GetMainPlayer().playerStats.id).alive && !stateDone)
        {
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player.playerStats.id != 0)
            {
                FlappyRoyaleGameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                stateDone = true;
            }
        }



        if (gameManager.actualDifficulty.difficulty != Difficulty.Passive && difficultyUpgradeTime > 0)
        {
            difficultyUpgradeTime -= Time.deltaTime;
            difficultyUpgradeTime = Mathf.Clamp(difficultyUpgradeTime, 0f, Mathf.Infinity);
        }
        preparationTime -= Time.deltaTime;
        preparationTime = Mathf.Clamp(preparationTime, 0f, Mathf.Infinity);
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