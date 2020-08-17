using UnityEngine;

[System.Serializable]
public class SideStats
{
    [SerializeField]
    private Collider sideCollider = null;
    [SerializeField]
    private float value = 0f;
    public Collider GetSideCollider()
    {
        return sideCollider;
    }
    public float GetValue()
    {
        return value;
    }
}