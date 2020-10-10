using UnityEngine;

[System.Serializable]
public class SideStats
{
    [SerializeField]
    public Collider sideCollider = null;
    [SerializeField]
    public float value = 0f;
    public Collider GetSideCollider()
    {
        return sideCollider;
    }
    public float GetValue()
    {
        return value;
    }
}