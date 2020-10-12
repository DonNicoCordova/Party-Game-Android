using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

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
        var orderingPhase = new OrderDecidingPhase(3f);
        phases.Add(orderingPhase);
        var orderingResultPhase = new OrderResultsPhase(3f);
        phases.Add(orderingResultPhase);
        var throwPhase = new ThrowPhase(3f);
        phases.Add(throwPhase);
        var throwResultsPhase = new ThrowResultsPhase(3f);
        phases.Add(throwResultsPhase);
        var movePiecePhase = new MovePiecePhase(3f, 30f);
        phases.Add(movePiecePhase);
        var moveResultsPhase = new MoveResultsPhase(3f);
        phases.Add(moveResultsPhase);
        var minigamePhase = new MinigamePhase(3f);
        phases.Add(minigamePhase);
        var finalResultsPhase = new FinalResultsPhase(3f);
        phases.Add(finalResultsPhase);

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
            Debug.Log("CHECKING IF ORDERING DONE");
            Debug.Log($"PLAYERSSETANDORDERED(): {GameManager.instance.PlayersSetAndOrdered()} ALLPLAYERSSTATEDONE: {GameManager.instance.AllPlayersStateDone()}");
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
}