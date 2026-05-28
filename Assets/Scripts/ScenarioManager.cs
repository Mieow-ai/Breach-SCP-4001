using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int dossiersCollectes = 0;
    public int totalDossiers = 5;

    [Header("UI")]
    public TextMeshProUGUI livreCounterText;

    [Header("Audio")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return; // Stoppe l'exécution du Awake pour l'objet détruit
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        MettreAJourUI();
    }

    public void AjouterDossier()
    {
        dossiersCollectes++;
        MettreAJourUI();

        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);
    }

    public bool TousDossiersRecuperes()
    {
        return dossiersCollectes >= totalDossiers;
    }

    private void MettreAJourUI()
    {
        if (livreCounterText != null)
        {
            livreCounterText.text = $"Vous avez récupéré {dossiersCollectes} / {totalDossiers} livres";
        }
    }
}
