using UnityEngine;

public class PickupBattery : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode pickupKey = KeyCode.E;
    public GameObject promptObject; // "Appuyez sur E pour prendre"

    [Header("Inventory")]
    public BatteryInventory batteryInventory; // Référence au système de stockage

    [Header("Sound")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    private bool playerInRange = false;
    private bool hasBeenPicked = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPicked)
        {
            playerInRange = true;
            if (promptObject) promptObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPicked)
        {
            playerInRange = false;
            if (promptObject) promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(pickupKey) && !hasBeenPicked)
        {
            hasBeenPicked = true;

            if (promptObject) promptObject.SetActive(false);

            Debug.Log("Batterie ramassée : " + gameObject.name);

            // Ajouter la batterie à l’inventaire
            if (batteryInventory != null)
                batteryInventory.AddBatteryToInventory();

            // Jouer le son
            if (pickupSound != null && audioSource != null)
                audioSource.PlayOneShot(pickupSound);

            // Détruire l’objet après la durée du son pour qu’il s’entende entièrement
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }
}



