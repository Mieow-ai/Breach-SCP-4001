using UnityEngine;
using System.Collections;

public class SCP096Attack : MonoBehaviour
{
    public Animator animator;             // Trigger "Attack"
    public PlayerHealth playerHealth;     // Référence au script de santé du joueur
    public float attackDelay = 1.5f;      // délai avant mort
    [HideInInspector] public bool canAttack = false; // activé par SCP096Behavior pendant la charge

    [Header("Screamer / Son")]
    public AudioClip screamSound;
    public AudioSource audioSource;
    public Transform faceTarget; // point vers lequel on force la caméra du joueur

    [Header("Références utiles")]
    public SCP096Behavior behaviorScript;
    public AmbianceManager ambianceManager;
    public SCP096Screamer screamerController;

    private bool hasAttacked = false;

    void Start()
    {
        if (behaviorScript == null)
            behaviorScript = GetComponentInParent<SCP096Behavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasAttacked || !canAttack) return;
        if (!other.CompareTag("Player")) return;

        hasAttacked = true;

        // Forcer extinction/lock de la lampe
        FlashlightToggle flashlight = other.GetComponentInParent<FlashlightToggle>();
        if (flashlight != null)
            flashlight.ForceTurnOffAndLock();

        // Screamer
        if (screamerController != null)
            screamerController.PlayScreamer();

        // Animation d'attaque
        if (animator != null)
            animator.SetTrigger("Attack");

        // Stopper le mouvement de 096 (sécurité)
        if (behaviorScript != null)
            behaviorScript.StopMovement();

        // Stopper ambiance rage si présent
        if (ambianceManager != null)
            ambianceManager.StopRageSound();

        // Jouer cri
        if (audioSource != null && screamSound != null)
            audioSource.PlayOneShot(screamSound);

        // Forcer la caméra du joueur à regarder 096
        PlayerMovementWithFreelook player = other.GetComponentInParent<PlayerMovementWithFreelook>();
        if (player != null)
            player.StartLookAt(faceTarget != null ? faceTarget : transform);

        // Tuer le joueur après delay
        if (playerHealth != null)
            StartCoroutine(DelayedKill());
    }

    private IEnumerator DelayedKill()
    {
        yield return new WaitForSeconds(attackDelay);
        if (playerHealth != null)
            playerHealth.Die();
    }
}
