using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoController : MonoBehaviour
{
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI zonesCapturedText;
    public Image playerColor;
    private PlayerStats associatedPlayer;
    public void InitializeFromPlayer(PlayerStats playerStats)
    {
        nicknameText.text = playerStats.nickName;
        zonesCapturedText.text = playerStats.GetCapturedZones().ToString();
        playerColor.material = playerStats.mainColor;
        playerStats.CapturedZone += (sender, args) => UpdateCapturedZones(args.NewCapturedZones);
        associatedPlayer = playerStats;
    }

    public void UpdateCapturedZones(float amount)
    {
        zonesCapturedText.text = amount.ToString();
    }

    private void OnDestroy()
    {
        associatedPlayer.CapturedZone -= (sender, args) => UpdateCapturedZones(args.NewCapturedZones);
    }
}
