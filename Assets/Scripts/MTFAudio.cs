using UnityEngine;
using UnityEngine.UI; // ou TMPro si tu utilises TextMeshPro

public class SoldierAudioWithUI : MonoBehaviour
{
    public AudioSource ambianceAudio;
    public AudioSource voiceAudio;
    public AudioClip[] voiceLines;

    public GameObject interactionPrompt; // UI à afficher
    private bool playerInRange = false;
    private bool isTalking = false;

    void Update()
    {
        if (playerInRange && !isTalking)
        {
            interactionPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
        else
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Interact()
    {
        isTalking = true;
        interactionPrompt.SetActive(false);

        if (ambianceAudio.isPlaying)
            ambianceAudio.Stop();

        if (voiceLines.Length > 0)
        {
            AudioClip clip = voiceLines[Random.Range(0, voiceLines.Length)];
            voiceAudio.clip = clip;
            voiceAudio.Play();
            Invoke(nameof(ResumeAmbiance), clip.length + 0.5f);
        }
        else
        {
            ResumeAmbiance();
        }
    }

    public MonsterAI monsterAI; // assigner via l’inspecteur

    void ResumeAmbiance()
    {
        ambianceAudio.Play();
        isTalking = false;
        GameSettings.Instance.introDone = true;
        if (monsterAI != null)
        {
            monsterAI.AllowChase();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionPrompt.SetActive(false);
        }
    }
}
