using UnityEngine;

public class SpawnCommand : CommandMono
{
    private Vector3 _spawnPosition;
    private bool _finished;
    private bool _failed;
    public SpawnCommand(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
    }
    public override void Reset()
    {
        _finished = false;
        _failed = false;
    }
    public override void Execute()
    {
        if (GameManager.Instance.saw != null)
        {
            GameManager.Instance.saw.transform.position = _spawnPosition;
        } else
        {
            _failed = true;
        }
        _finished = true;
    }
    public override bool IsFinished => _finished;
    public override bool Failed => _failed;
}
