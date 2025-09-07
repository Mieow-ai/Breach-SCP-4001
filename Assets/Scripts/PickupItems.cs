using UnityEngine;

public class PickupLetter : MonoBehaviour
{
    public Sprite letterSprite; // Assigner l'image de la feuille
    public KeyCode pickupKey = KeyCode.E;
    public GameObject promptObject; // TMP ou TextMeshPro pour "E pour ramasser"

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

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(pickupKey) && !hasBeenPicked)
        {
            hasBeenPicked = true;

            if (promptObject) promptObject.SetActive(false);

            // Passe l'image au gestionnaire
            LetterReader.Instance.SetLetterSprite(letterSprite);
            LetterReader.Instance.ShowReadPromptOnce();

            // Jouer le son
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            // Détruire l’objet après la fin du son
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }
}

