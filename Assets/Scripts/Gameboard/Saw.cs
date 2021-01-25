using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
public class Saw : MonoBehaviourPunCallbacks
{
    public Animator animator;

    private Queue<CommandMono> _commands = new Queue<CommandMono>();
    private CommandMono _currentCommand;

    private void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        ProcessCommands();
    }
    public void ProcessCommands()
    {
        if (_currentCommand != null && _currentCommand.IsFinished == false)
            return;

        if (_commands.Any() == false)
            return;
        _currentCommand = _commands.Dequeue();
        _currentCommand.Execute();
        if (_currentCommand.Failed)
        {
            _currentCommand.Reset();
            _commands.Enqueue(_currentCommand);
            _currentCommand = null;
        }
    }
    [PunRPC]
    public void Cut() 
    {
        CutCommand cutCommand = new CutCommand();
        _commands.Enqueue(cutCommand);
    }

    [PunRPC]
    public void SpawnAt(float positionX, float positionY, float positionZ)
    {
        Vector3 newPosition = new Vector3(positionX, positionY, positionZ);
        SpawnCommand spawnCommand = new SpawnCommand(newPosition);
        _commands.Enqueue(spawnCommand);
    }
}
