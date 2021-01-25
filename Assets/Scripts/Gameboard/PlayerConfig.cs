using UnityEngine;
using System;

[System.Serializable]
public class PlayerConfig
{
    [SerializeField]
    public Material mainColor;
    [SerializeField]
    public Material orbColor;
    [SerializeField]
    public LocationController startingLocation;
}