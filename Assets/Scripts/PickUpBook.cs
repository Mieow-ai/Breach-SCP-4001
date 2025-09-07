using UnityEngine;

public class PickupBook : MonoBehaviour
{
    public KeyCode pickupKey = KeyCode.E;
    public GameObject promptObject; // Pour afficher "Appuyez sur E pour prendre"

    [Header("Sound")]
    public AudioClip pickupSound;       // Son à jouer lors du ramassage
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

            Debug.Log("Livre ramassé : " + gameObject.name);

            // Jouer le son
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            // Prévenir le GameManager
            GameManager.Instance.AjouterDossier();

            // Détruire l’objet après la fin du son pour qu’il soit joué complètement
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }
}

