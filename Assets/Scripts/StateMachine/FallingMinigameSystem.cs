using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class FallingMinigameSystem : MonoBehaviour
{
    public static FallingMinigameSystem Instance;
    private StateMachine _stateMachine;
    private GameManager _gameManager;
    private List<IState> phases = new List<IState>();

    private void Awake()
    {

        if (Instance != null)
        {
            return;
        }
        Instance = this;
        _gameManager = GameManager.Instance;
        _stateMachine = new StateMachine();

        var instructionsPhase = new FallingInstructionsPhase(10f);
        phases.Add(instructionsPhase);
        var playPhase = new FallingPlayPhase();
        phases.Add(playPhase);
        var gameOverPhase = new FallingMinigameOverPhase(5f);
        phases.Add(gameOverPhase);

        At(instructionsPhase, playPhase, AllPlayersReady());
        At(playPhase, gameOverPhase, DonePlaying());
        
        StartCoroutine(Setup(instructionsPhase));
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }
        
    Func<bool> AllPlayersReady() => () =>
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return GameManager.Instance.AllPlayersMinigameStateDone();
        }
        else
        {
            return false;
        }
    };

    Func<bool> DonePlaying() => () =>
    {
        //Change to make sure the minigame has ended, for now checks the same as gameboard phase done
        if (PhotonNetwork.IsMasterClient)
        {
            return FallingGameManager.Instance.AllPlayersSpawnerDone();
        }
        else
        {
            return false;
        }
    };

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