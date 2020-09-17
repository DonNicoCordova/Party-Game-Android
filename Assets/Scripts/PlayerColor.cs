using UnityEngine;
using System;

[System.Serializable]
public class PlayerColor
{
    [SerializeField]
    public Material mainColor;
    [SerializeField]
    public Material orbColor;
    [SerializeField]
    public LocationController startingLocation;
}