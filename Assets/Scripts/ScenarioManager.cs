using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int dossiersCollectes = 0;
    public int totalDossiers = 5;

    [Header("UI")]
    public TextMeshProUGUI livreCounterText;

    [Header("Audio")]
    public AudioClip pickupSound;       // 🎵 le son de ramassage
    private AudioSource audioSource;    // 🎧 pour le jouer

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Récupère (ou ajoute) un AudioSource sur ce GameObject
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

        // Mets à jour le texte
        MettreAJourUI();

        // Joue le son si défini
        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);

        Debug.Log("Dossiers récupérés : " + dossiersCollectes + "/" + totalDossiers);
    }

    public bool TousDossiersRecuperes()
    {
        return dossiersCollectes >= totalDossiers;
    }

    private void MettreAJourUI()
    {
        if (livreCounterText != null)
        {
            livreCounterText.text = "Vous avez recupere "
                + dossiersCollectes + " / " + totalDossiers + " livres";
        }
    }
}
