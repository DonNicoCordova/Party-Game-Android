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
    public List<SideStats> throwValues = new List<SideStats>();
    public bool isMainPlayer = false;

    public int GetValue()
    {
        if (throwValues.Count == 0) 
        {
            return Random.Range(2, 12);
        };
        int output = 0;
        foreach (SideStats stat in throwValues)
        {
            output += (int)stat.GetValue();
        }
        return output;
    }
}