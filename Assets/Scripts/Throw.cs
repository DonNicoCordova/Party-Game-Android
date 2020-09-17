using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Throw
{
    [SerializeField]
    public PlayerStats player;
    [SerializeField]
    public string playerNickname;
    [SerializeField]
    public List<SideStats> throwValues = new List<SideStats>();

    public Throw(PlayerStats throwPlayer)
    {
        player = throwPlayer;
    }
    public int GetValue()
    {
        if (throwValues.Count == 0) 
        {
            return Random.Range(2, 12);
        };
        int output = 0;
        foreach (SideStats stat in throwValues)
        {
            output += 7-(int)stat.GetValue();
        }
        return output;
    }
}