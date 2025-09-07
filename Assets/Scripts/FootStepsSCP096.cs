using UnityEngine;

public class FootstepSCP096 : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    public AudioSource audioSource;

    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private float currentStepInterval;
    private float stepTimer;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentStepInterval = walkStepInterval;
        stepTimer = 0f;
    }

    void Update()
    {
        if (animator == null || audioSource == null)
            return;

        // Définir l'intervalle de pas selon l'animation en cours
        bool isCharging = animator.GetBool("IsCharging");
        bool isMoving = animator.GetBool("IsMoving");

        if (isCharging)
            currentStepInterval = runStepInterval;
        else if (isMoving)
            currentStepInterval = walkStepInterval; // ou un autre intervalle si tu veux un pas différent en colère mais pas en charge
        else
            stepTimer = 0f; // Pas de pas si idle

        // On joue les sons de pas seulement si l'animator est en marche ou en charge
        if (isCharging || isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= currentStepInterval)
            {
                PlayFootstepSound();
                stepTimer = 0f;
            }
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0) return;
        int index = Random.Range(0, footstepSounds.Length);
        audioSource.PlayOneShot(footstepSounds[index]);
    }
}
