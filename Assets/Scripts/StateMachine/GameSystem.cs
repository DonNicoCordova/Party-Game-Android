using System;
using UnityEngine;

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
        _stateMachine.SetState(initialize);

        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

        Func<bool> orderingThrowFinished() => () => GameManager.instance.throwController.IsThrowFinished() && orderingPhaseDone;
        Func<bool> throwFinished() => () => GameManager.instance.throwController.IsThrowFinished() && throwPhaseDone;
        Func<bool> resultsDone() => () => throwResultsPhaseDone;
        Func<bool> orderNotDefined() => () => !GameManager.instance.PlayersSetAndOrdered() && initializePhaseDone;
        Func<bool> orderDefined() => () => GameManager.instance.PlayersSetAndOrdered() && initializePhaseDone;
        Func<bool> orderingDone() => () => GameManager.instance.PlayersSetAndOrdered() && orderingResultsPhaseDone;
        Func<bool> nothingElseToDo() => () => GameManager.instance.RoundDone() && movePiecePhaseDone;
        Func<bool> nextRoundReady() => () => GameManager.instance.NextRoundReady() && moveResultsPhaseDone;
    }

    private void Update() => _stateMachine.Tick();
    private void FixedUpdate() => _stateMachine.FixedTick();
}