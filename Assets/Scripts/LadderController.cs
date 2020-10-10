using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class LadderController : MonoBehaviour
{
    public GameObject playerInfoPrefab;

    public PlayerInfoContainer[] playerInfoContainers;
    public void Initialize()
    {
        for(int i = 0; i < playerInfoContainers.Length; ++i)
        {
            PlayerInfoContainer container = playerInfoContainers[i];
            if (i < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                PlayerController controller = GameManager.instance.GetPlayer(PhotonNetwork.PlayerList[i].ActorNumber);
                container.InitializeFromPlayer(controller);
            } else
            {
                container.obj.SetActive(false);
            }
        }
        gameObject.SetActive(true);
        //foreach (PlayerController playerStats in players)
        //{
        //    GameObject newInfoPrefab = Instantiate<GameObject>(playerInfoPrefab, transform);
        //    PlayerInfoController controller = newInfoPrefab.GetComponent<PlayerInfoController>();
        //    controller.InitializeFromPlayer(playerStats);
        //}
    }
    
}

[System.Serializable]
public class PlayerInfoContainer
{
    public GameObject obj;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI zonesCapturedText;
    public Image playerColor;
    private PlayerController associatedPlayer;
    public void InitializeFromPlayer(PlayerController player)
    {
        nicknameText.text = player.playerStats.nickName;
        zonesCapturedText.text = player.playerStats.capturedZones.ToString();
        playerColor.material = player.playerStats.mainColor;
        player.playerStats.CapturedZone += (sender, args) => UpdateCapturedZones();
        associatedPlayer = player;
    }

    public void UpdateCapturedZones()
    {
        zonesCapturedText.text = associatedPlayer.playerStats.capturedZones.ToString();
    }

    private void OnDestroy()
    {
        associatedPlayer.playerStats.CapturedZone -= (sender, args) => UpdateCapturedZones();
    }
}
