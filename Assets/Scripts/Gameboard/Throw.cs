using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Throw
{
    [SerializeField]
    public int playerId;
    [SerializeField]
    public string playerNickname;
    [SerializeField]
    public int throwValue = 0;

    public Throw(int newPlayerId)
    {
        playerId = newPlayerId;
    }
}