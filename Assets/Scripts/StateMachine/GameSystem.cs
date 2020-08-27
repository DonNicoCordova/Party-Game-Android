using System;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem instance;
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

        var initialize = new Initialize();
        var orderingPhase = new OrderDecidingPhase();
        var orderingResultPhase = new OrderReultsPhase();
        var throwPhase = new ThrowPhase();
        var throwResultsPhase = new ThrowResultsPhase();
        var movePiecePhase = new MovePiecePhase();
        var moveResultsPhase = new MoveResultsPhase();
        var minigamePhase = new MinigamePhase();
        var finalResultsPhase = new FinalResultsPhase();

        At(initialize, orderingPhase, orderNotDefined());
        At(initialize, throwPhase, orderDefined());
        At(orderingPhase, orderingResultPhase, throwFinished());
        At(orderingResultPhase, initialize, playerQueueReady());
        At(throwPhase, throwResultsPhase, throwFinished());
        At(throwResultsPhase, movePiecePhase, yourTurn());
        At(movePiecePhase, moveResultsPhase, nothingElseToDo());
        At(moveResultsPhase, initialize, nextRoundReady());
        _stateMachine.SetState(initialize);

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
        Func<bool> orderDefined() => () => GameManager.instance.throwController.DicesReadyToPlay() && GameManager.instance.PlayersSet();
        Func<bool> throwFinished() => () => GameManager.instance.throwController.IsThrowFinished();
        Func<bool> yourTurn() => () => GameManager.instance.YourTurn();
        Func<bool> orderNotDefined() => () => !GameManager.instance.PlayersSet() && GameManager.instance.throwController.DicesReadyToPlay();
        Func<bool> playerQueueReady() => () => GameManager.instance.PlayersSet();
        Func<bool> nothingElseToDo() => () => GameManager.instance.NothingElseToDo();
        Func<bool> nextRoundReady() => () => GameManager.instance.NextRoundReady();
    }

    private void Update() => _stateMachine.Tick();

}