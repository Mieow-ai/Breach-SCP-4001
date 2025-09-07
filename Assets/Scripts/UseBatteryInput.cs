using UnityEngine;

public class UseBatteryInput : MonoBehaviour
{
    [Header("Références")]
    public BatteryInventory batteryInventory;
    public CamcorderUIManager camcorderUIManager;
    public FlashlightToggle flashlightToggle;       // Référence au script de la lampe

    [Header("Paramètres")]
    public KeyCode useBatteryKey = KeyCode.B;
    public float rechargeAmount = 25f;

    void Update()
    {
        if (Input.GetKeyDown(useBatteryKey))
        {
            if (batteryInventory != null && camcorderUIManager != null)
            {
                // Recharge la batterie, peu importe si la torche est allumée ou non
                bool success = batteryInventory.UseBattery(camcorderUIManager, rechargeAmount);

                if (success)
                {
                    Debug.Log("Batterie utilisée ! Nouvelle quantité : " + batteryInventory.batteryCount);

                    // 🔦 Déverrouille la lampe si elle était bloquée
                    if (flashlightToggle != null)
                    {
                        flashlightToggle.Unlock();
                    }
                }
                else
                {
                    Debug.Log("Pas de batterie en stock !");
                }
            }
        }
    }
}



