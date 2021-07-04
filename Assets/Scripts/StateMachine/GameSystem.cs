using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class GameSystem : GenericSingletonClass<GameSystem>
{
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
    private List<IState> phases = new List<IState>();

    public override void Awake()
    {
        base.Awake();
        _stateMachine = new StateMachine();

        var initialize = new InitializeRound(2f);
        phases.Add(initialize);
        var welcome = new WelcomePhase(2f);
        phases.Add(initialize);
        var orderingPhase = new OrderDecidingPhase(3f);
        phases.Add(orderingPhase);
        var orderingResultPhase = new OrderResultsPhase(2f);
        phases.Add(orderingResultPhase);
        var throwPhase = new ThrowPhase(15f);
        phases.Add(throwPhase);
        var throwResultsPhase = new ThrowResultsPhase(2f);
        phases.Add(throwResultsPhase);
        var movePiecePhase = new MovePiecePhase(3f, 60f);
        phases.Add(movePiecePhase);
        var moveResultsPhase = new MoveResultsPhase(2f);
        phases.Add(moveResultsPhase);
        var minigamePhase = new MinigamePhase(90f);
        phases.Add(minigamePhase);
        var resumePhase = new ResumePhase(2f);
        phases.Add(resumePhase);
        var minigameResultsPhase = new MinigameResultsPhase(6f);
        phases.Add(minigameResultsPhase);
        var finalResultsPhase = new FinalResultsPhase(2f);
        phases.Add(finalResultsPhase);
        var gameOverPhase = new GameOverPhase(2f);
        phases.Add(gameOverPhase);

        At(welcome, orderingPhase, orderNotDefined());
        At(initialize, throwPhase, orderDefined());

        At(orderingPhase, initialize, orderingDone());
        At(throwPhase, throwResultsPhase, throwFinished());
        At(throwResultsPhase, movePiecePhase, resultsDone());
        At(movePiecePhase, moveResultsPhase, nothingElseToDo());
        At(moveResultsPhase, minigamePhase, gameboardPhaseDone());
        At(minigamePhase, resumePhase, minigameOver());
        At(resumePhase, minigameResultsPhase, timerDone());
        At(minigameResultsPhase, gameOverPhase, finalRoundFinished());
        At(minigameResultsPhase, orderingPhase, nextRoundReady());

        StartCoroutine(Setup(welcome));
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

        Func<bool> throwFinished() => () => {

            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.AllPlayersThrown() && GameManager.Instance.AllPlayersStateDone();
            }
            else
                return false;
        };
        Func<bool> resultsDone() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.AllPlayersStateDone();
            }
            else
                return false;
        };
        Func<bool> orderNotDefined() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return !GameManager.Instance.PlayersSetAndOrdered() && GameManager.Instance.AllPlayersStateDone();
            }
            else
                return false;
        };
        Func<bool> orderDefined() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            { 
                return GameManager.Instance.PlayersSetAndOrdered() && GameManager.Instance.AllPlayersStateDone();
            }
            else
                return false;
        };
        Func<bool> orderingDone() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.PlayersSetAndOrdered() && GameManager.Instance.AllPlayersStateDone();
            }
            else
                return false;
        };
        Func<bool> timerDone() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.AllPlayersStateDone();
            }
            else
                return false;
        };
        Func<bool> nothingElseToDo() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.RoundDone() && GameManager.Instance.AllPlayersStateDone();
            }
            else
                return false;
        };
        Func<bool> gameboardPhaseDone() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.GameBoardDone() && GameManager.Instance.AllPlayersStateDone();
            } else
            {
                return false;
            }
        };
        Func<bool> nextRoundReady() => () =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.NextRoundReady() && GameManager.Instance.AllPlayersStateDone() && GameManager.Instance.AllPlayersCharacterSpawned() && GameManager.Instance.miniGamesQueue.Count > 0;
            }
            else
            {
                return false;
            }
        }; 
        Func<bool> minigameOver() => () =>
        {
            //Change to make sure the minigame has ended, for now checks the same as gameboard phase done
            if (PhotonNetwork.IsMasterClient)
            {
                return GameManager.Instance.GameBoardDone() && GameManager.Instance.AllPlayersMinigameOver() && GameManager.Instance.AllPlayersMinigameStateDone();
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
                return GameManager.Instance.NextRoundReady() && GameManager.Instance.AllPlayersStateDone() && (GameManager.Instance.GetRound() == GameManager.Instance.maxRounds || GameManager.Instance.miniGamesQueue.Count == 0);
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
        while (!GameManager.Instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
        _stateMachine.SetState(initialState);
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