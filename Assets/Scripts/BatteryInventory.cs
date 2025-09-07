using UnityEngine;
using TMPro;

public class BatteryInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int batteryCount = 0;

    [Header("UI")]
    public TextMeshProUGUI batteryCounterText; // TMP Text sur le Canvas

    private void Start()
    {
        UpdateBatteryUI();
    }

    // Ajouter une batterie
    public void AddBatteryToInventory()
    {
        batteryCount++;
        UpdateBatteryUI();
    }

    // Utiliser une batterie pour recharger la torche
    public bool UseBattery(CamcorderUIManager camcorderUI, float rechargeAmount = 25f)
    {
        if (batteryCount <= 0) return false;

        batteryCount--;
        UpdateBatteryUI();

        if (camcorderUI != null)
        {
            camcorderUI.RechargeBattery(rechargeAmount);
        }

        return true;
    }

    // Met à jour le texte sur l’UI
    private void UpdateBatteryUI()
    {
        if (batteryCounterText != null)
            batteryCounterText.text = $"Batteries : {batteryCount}";
    }
}


