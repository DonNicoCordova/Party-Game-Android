using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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

        var beginning = new Begin(_gameManager);
        var throwPhase = new ThrowPhase(_gameManager);
        var throwResultsPhase = new ThrowResultsPhase(_gameManager);
        var movePiecePhase = new MovePiecePhase();
        var moveResultsPhase = new MoveResultsPhase();
        var minigamePhase = new MinigamePhase();
        var finalResultsPhase = new FinalResultsPhase();

        At(beginning, throwPhase, dicesReadyToPlay());
        At(throwPhase, throwResultsPhase, throwFinished());
        At(throwResultsPhase, movePiecePhase, diceOnDisplay());
        _stateMachine.SetState(beginning);

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
        Func<bool> dicesReadyToPlay() => () => GameManager.instance.throwController.DicesReadyToPlay();
        Func<bool> throwFinished() => () => GameManager.instance.throwController.IsThrowFinished();
        Func<bool> diceOnDisplay() => () => GameManager.instance.DiceOnDisplay();
    }

    private void Update() => _stateMachine.Tick();


}