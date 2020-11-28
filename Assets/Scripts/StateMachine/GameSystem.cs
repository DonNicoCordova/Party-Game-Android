using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class GameSystem : MonoBehaviour
{
    public static GameSystem instance;
    public Boolean initializePhaseTimerDone = false;
    public Boolean orderingPhaseTimerDone = false;
    public Boolean orderingResultsPhaseTimerDone = false;
    public Boolean throwPhaseTimerDone = false;
    public Boolean throwResultsPhaseTimerDone = false;
    public Boolean movePiecePhaseTimerDone = false;
    public Boolean moveResultsPhaseTimerDone = false;
    public Boolean minigamePhaseTimerDone = false;
    public Boolean finalResultsPhaseTimerDone = false;
    public Boolean gameOverPhaseTimerDone = false;
    private StateMachine _stateMachine;
    private GameManager _gameManager;
    private List<IState> phases = new List<IState>();

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
        phases.Add(initialize);
        var orderingPhase = new OrderDecidingPhase(15f);
        phases.Add(orderingPhase);
        var orderingResultPhase = new OrderResultsPhase(3f);
        phases.Add(orderingResultPhase);
        var throwPhase = new ThrowPhase(15f);
        phases.Add(throwPhase);
        var throwResultsPhase = new ThrowResultsPhase(3f);
        phases.Add(throwResultsPhase);
        var movePiecePhase = new MovePiecePhase(3f, 60f);
        phases.Add(movePiecePhase);
        var moveResultsPhase = new MoveResultsPhase(3f);
        phases.Add(moveResultsPhase);
        var minigamePhase = new MinigamePhase(90f);
        phases.Add(minigamePhase);
        var finalResultsPhase = new FinalResultsPhase(3f);
        phases.Add(finalResultsPhase);
        var gameOverPhase = new GameOverPhase(3f);
        phases.Add(gameOverPhase);

        At(initialize, orderingPhase, orderNotDefined());
        At(initialize, throwPhase, orderDefined());
        At(orderingPhase, orderingResultPhase, orderingThrowFinished());
        At(orderingResultPhase, initialize, orderingDone());
        At(throwPhase, throwResultsPhase, throwFinished());
        At(throwResultsPhase, movePiecePhase, resultsDone());
        At(movePiecePhase, moveResultsPhase, nothingElseToDo());
        At(moveResultsPhase, minigamePhase, gameboardPhaseDone());
        At(minigamePhase, gameOverPhase, finalRoundFinished());
        At(minigamePhase, initialize, nextRoundReady());

        StartCoroutine(Setup(initialize));
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

        Func<bool> orderingThrowFinished() => () => {
            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.AllPlayersThrown() && GameManager.instance.AllPlayersStateDone();
            else
                return false;
        };
        Func<bool> throwFinished() => () => {

            if (PhotonNetwork.IsMasterClient)
                return GameManager.instance.AllPlayersThrown() && GameManager.instance.AllPlayersStateDone();
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
        Func<bool> gameboardPhaseDone() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.instance.NextRoundReady() && GameManager.instance.AllPlayersStateDone();
            } else
            {
                return false;
            }
        };
        Func<bool> nextRoundReady() => () =>
        {
            //Change to make sure the minigame has ended, for now checks the same as gameboard phase done
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.instance.NextRoundReady() && GameManager.instance.AllPlayersStateDone();
            }
            else
            {
                return false;
            }
        };
        Func<bool> finalRoundFinished() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.instance.NextRoundReady() && GameManager.instance.AllPlayersStateDone() && GameManager.instance.GetRound() == GameManager.instance.maxRounds;
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
        while (!GameManager.instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
        _stateMachine.SetState(initialState);
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(string state)
    {
        foreach (IState phase in phases)
        {
            if (phase.GetType().Name == state)
            {
                _stateMachine.SetState(phase);
                break;
            }
        }
    }
    public string GetCurrentStateName()
    {
        if (_stateMachine.GetCurrentState() != null)
        {
            return _stateMachine?.GetCurrentState().GetType().Name;
        } else
        {
            return "None";
        }
    }
}