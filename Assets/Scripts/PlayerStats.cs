using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [SerializeField]
    public int id;
    [SerializeField]
    public float capturedZones;
    [SerializeField]
    public float ladderPosition = 1;
    [SerializeField]
    public Color mainColor;
    [SerializeField]
    public int money = 0;
    [SerializeField]
    public int mana = 20;
    [SerializeField]
    public string nickName;
    [SerializeField]
    public bool isPlayer = false;
    [SerializeField]
    public bool moved = false;
    [SerializeField]
    public bool usedSkill = false;
    [SerializeField]
    public bool passed = false;
}