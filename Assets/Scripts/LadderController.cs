using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    public GameObject playerInfoPrefab;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void Initialize(PlayerStats[] players)
    {
        foreach (PlayerStats playerStats in players)
        {
            GameObject newInfoPrefab = Instantiate<GameObject>(playerInfoPrefab, transform);
            PlayerInfoController controller = newInfoPrefab.GetComponent<PlayerInfoController>();
            controller.InitializeFromPlayer(playerStats);
        }
    }
}
