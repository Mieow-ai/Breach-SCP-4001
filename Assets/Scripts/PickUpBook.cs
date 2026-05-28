using UnityEngine;

public class PickupBook : MonoBehaviour
{
    public KeyCode pickupKey = KeyCode.E;
    public GameObject promptObject;

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

            if (pickupSound != null && audioSource != null)
                audioSource.PlayOneShot(pickupSound);

            GameManager.Instance.AjouterDossier();

            HideAndDisableObject();
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }

    private void HideAndDisableObject()
    {
        foreach (var rend in GetComponentsInChildren<Renderer>()) rend.enabled = false;
        foreach (var col in GetComponentsInChildren<Collider>()) col.enabled = false;
    }
}

