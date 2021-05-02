using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeModelEvents : MonoBehaviour
{
    private Bridge bridgeController;

    private void Start()
    {
        bridgeController = gameObject.GetComponentInParent<Bridge>();
    }

    private void ShowMap()
    {
        bridgeController.ShowMap();
    }
}
