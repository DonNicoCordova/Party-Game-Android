using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [SerializeField]
    public int id;
    [SerializeField]
    public float capturedZones;
    [SerializeField]
    public float ladderPosition;
    [SerializeField]
    public float mainColor;
    [SerializeField]
    public int money;
    [SerializeField]
    public int mana;
    [SerializeField]
    public string nickName;
    [SerializeField]
    public bool isPlayer;
}