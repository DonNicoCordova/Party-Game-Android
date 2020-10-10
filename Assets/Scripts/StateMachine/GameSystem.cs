using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GameSystem : MonoBehaviour
{
    public static GameSystem instance;
    public Boolean initializePhaseDone = false;
    public Boolean orderingPhaseDone = false;
    public Boolean orderingResultsPhaseDone = false;
    public Boolean throwPhaseDone = false;
    public Boolean throwResultsPhaseDone = false;
    public Boolean movePiecePhaseDone = false;
    public Boolean moveResultsPhaseDone = false;
    public Boolean minigamePhaseDone = false;
    public Boolean finalResultsPhaseDone = false;
    private StateMachine _stateMachine;
    private GameManager _gameManager;
    private IState initialize;
    private IState orderingPhase;
    private IState orderingResultPhase;
    private IState throwPhase;
    private IState throwResultsPhase;
    private IState movePiecePhase;
    private IState moveResultsPhase;
    private IState minigamePhase;
    private IState finalResultsPhase;

    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogError("More than one GameSystem in scene!");
            return;
        }
        instance = this;
        _gameManager = GameManager.instance;
        _stateMachine = new StateMachine();

        var initialize = new Initialize(3f);
        var orderingPhase = new OrderDecidingPhase(3f);
        var orderingResultPhase = new OrderResultsPhase(3f);
        var throwPhase = new ThrowPhase(3f);
        var throwResultsPhase = new ThrowResultsPhase(3f);
        var movePiecePhase = new MovePiecePhase(3f, 30f);
        var moveResultsPhase = new MoveResultsPhase(3f);
        var minigamePhase = new MinigamePhase(3f);
        var finalResultsPhase = new FinalResultsPhase(3f);

        At(initialize, orderingPhase, orderNotDefined());
        At(initialize, throwPhase, orderDefined());
        At(orderingPhase, orderingResultPhase, orderingThrowFinished());
        At(orderingResultPhase, initialize, orderingDone());
        At(throwPhase, throwResultsPhase, throwFinished());
        At(throwResultsPhase, movePiecePhase, resultsDone());
        At(movePiecePhase, moveResultsPhase, nothingElseToDo());
        At(moveResultsPhase, initialize, nextRoundReady());

        StartCoroutine(Setup(initialize));
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

        Func<bool> orderingThrowFinished() => () => {
            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.throwController.IsThrowFinished() && GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> throwFinished() => () => {

            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.throwController.IsThrowFinished() && GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> resultsDone() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> orderNotDefined() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
                return !GameManager.instance.PlayersSetAndOrdered() && GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> orderDefined() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.PlayersSetAndOrdered() && GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> orderingDone() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.PlayersSetAndOrdered() && GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> nothingElseToDo() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.RoundDone() && GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> nextRoundReady() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.instance.NextRoundReady() && GameManager.instance.AllPlayersStateDone();
            } else
            {
                return false;
            }
        }; 
    }

    private void Update() => _stateMachine.Tick();
    private void FixedUpdate() => _stateMachine.FixedTick();
    private IEnumerator Setup(IState initialState)
    {

        yield return new WaitForSeconds(0.5f);
        Debug.Log($"JOINED STATUS {GameManager.instance?.AllPlayersJoined()}");
        while (!GameManager.instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
        _stateMachine.SetState(initialState);
    }

    public void SetState(IState state)
    {
        _stateMachine.SetState(state);
    }
}