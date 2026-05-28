using UnityEngine;
using System.Collections;

public class SCP096Attack : MonoBehaviour
{
    public Animator animator;
    public PlayerHealth playerHealth;
    public float attackDelay = 1.5f;
    [HideInInspector] public bool canAttack = false;

    [Header("Screamer / Son")]
    public AudioClip screamSound;
    public AudioSource audioSource;
    public Transform faceTarget;

    [Header("Références utiles")]
    public SCP096Behavior behaviorScript;
    public AmbianceManager ambianceManager;
    public SCP096Screamer screamerController;

    private bool hasAttacked = false;
    
    // Optimisation Hash Animator
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    void Start()
    {
        if (behaviorScript == null)
            behaviorScript = GetComponentInParent<SCP096Behavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collision avec : {other.name} | Tag: {other.tag}");
        if (hasAttacked || !canAttack) return;
        if (!other.CompareTag("Player")) return;

        hasAttacked = true;

        // Mise en cache depuis la racine pour éviter plusieurs GetComponent coûteux
        Transform playerRoot = other.transform.root;

        FlashlightToggle flashlight = playerRoot.GetComponentInChildren<FlashlightToggle>();
        if (flashlight != null) flashlight.ForceTurnOffAndLock();

        if (screamerController != null) screamerController.PlayScreamer();
        if (animator != null) animator.SetTrigger(AttackHash);
        if (behaviorScript != null) behaviorScript.StopMovement();
        if (ambianceManager != null) ambianceManager.StopRageSound();
        if (audioSource != null && screamSound != null) audioSource.PlayOneShot(screamSound);

        PlayerMovementWithFreelook player = playerRoot.GetComponentInChildren<PlayerMovementWithFreelook>();
        if (player != null) player.StartLookAt(faceTarget != null ? faceTarget : transform);

        if (playerHealth != null) StartCoroutine(DelayedKill());
    }

    private IEnumerator DelayedKill()
    {
        yield return new WaitForSeconds(attackDelay);
        if (playerHealth != null) playerHealth.Die();
    }
}
