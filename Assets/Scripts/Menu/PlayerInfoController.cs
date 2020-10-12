using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoController : MonoBehaviour
{
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI zonesCapturedText;
    public Image playerColor;
    private PlayerController associatedPlayer;
    public void InitializeFromPlayer(PlayerController player)
    {
        nicknameText.text = player.playerStats.nickName;
        zonesCapturedText.text = player.playerStats.GetCapturedZones().ToString();
        playerColor.material = player.playerStats.mainColor;
        player.playerStats.CapturedZone += (sender, args) => UpdateCapturedZones(args.NewCapturedZones);
        associatedPlayer = player;
    }

    public void UpdateCapturedZones(float amount)
    {
        zonesCapturedText.text = amount.ToString();
    }

    private void OnDestroy()
    {
        associatedPlayer.playerStats.CapturedZone -= (sender, args) => UpdateCapturedZones(args.NewCapturedZones);
    }
}
