using UnityEngine;

public class FlashlightUIController : MonoBehaviour
{
    public GameObject flashlightAvailableUI;

    public void ShowAvailableUI()
    {
        if (flashlightAvailableUI != null)
            flashlightAvailableUI.SetActive(true);
    }

    public void HideUI()
    {
        if (flashlightAvailableUI != null)
            flashlightAvailableUI.SetActive(false);
    }
}
