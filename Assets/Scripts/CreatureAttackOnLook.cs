using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SCP096Behavior : MonoBehaviour
{
    public Transform playerCamera;
    public float viewAngleThreshold = 15f;
    public float maxDetectionDistance = 20f;
    public SCP096Attack attackScript;
    public Animator animator;
    public NavMeshAgent agent;
    public AmbianceManager ambianceManager;
    public RageLightsController rageLightsController;
    public FlashlightUIController flashlightUIController;
    public float chargeSpeed = 10f;
    public float normalSpeed = 3.5f;

    public float phase2FreezeDuration = 3f; // temps avant que joueur puisse bouger
    public float phase2TotalDuration = 35f; // durée totale de crise

    private enum State { Idle, Angry, Charging }
    private State currentState = State.Idle;

    private PlayerMovementWithFreelook playerScript;
    public Transform faceTarget;
    public Collider faceCollider;

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        agent.updateRotation = false;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.radius = 0.5f; // ajuster selon la taille de SCP pour éviter collisions mur
        agent.height = 3f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        if (playerCamera != null)
            playerScript = playerCamera.GetComponentInParent<PlayerMovementWithFreelook>();
    }

    private void Update()
    {
        if (currentState == State.Charging)
        {
            if (agent.enabled && !agent.isStopped)
                agent.SetDestination(playerCamera.position);
            return;
        }

        if (currentState == State.Idle)
        {
            if (IsPlayerLooking())
            {
                StartCoroutine(TriggeredSequence());
            }
        }

        if (agent.enabled && !agent.isStopped && currentState != State.Charging)
            HandleRotation();
    }

    private void HandleRotation()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0;
        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    bool IsPlayerLooking()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDetectionDistance))
        {
            if (hit.collider == faceCollider)
                return true;
        }
        return false;
    }

    private IEnumerator TriggeredSequence()
    {
        currentState = State.Angry;

        // Phase 2 : freeze complet
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.enabled = false;
        animator.SetBool("IsAngry", true);

        if (playerScript)
            playerScript.StartLookAt(faceTarget != null ? faceTarget : transform);

        var flashlight = playerCamera.GetComponentInParent<FlashlightToggle>();
        if (flashlight)
        {
            flashlight.ForceTurnOffAndLock();
            if (flashlightUIController) flashlightUIController.HideUI();
        }

        if (rageLightsController) rageLightsController.SetRageState(true);
        if (ambianceManager) ambianceManager.PlayRageAmbience();

        yield return new WaitForSeconds(phase2FreezeDuration);
        if (playerScript) playerScript.StopLookAt();

        yield return new WaitForSeconds(phase2TotalDuration - phase2FreezeDuration);

        // Phase 3 : charge
        currentState = State.Charging;
        animator.SetBool("IsAngry", false);
        animator.SetBool("IsCharging", true);
        if (rageLightsController) rageLightsController.SetRageState(false);

        agent.enabled = true;
        agent.isStopped = false;
        agent.speed = chargeSpeed;
        agent.updateRotation = true;
        agent.angularSpeed = 1200f; // rotation rapide pour suivre le joueur
        agent.acceleration = 50f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        if (attackScript) attackScript.canAttack = true;

        if (flashlight)
        {
            flashlight.Unlock();
            if (flashlightUIController) flashlightUIController.ShowAvailableUI();
        }

        while (currentState == State.Charging)
        {
            agent.SetDestination(playerCamera.position);
            yield return new WaitForSeconds(0.05f); // plus réactif pour éviter collisions
        }
    }

    public void StopMovement()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    public void ResumeMovement()
    {
        if (agent.enabled && currentState == State.Idle)
        {
            agent.speed = normalSpeed;
            agent.isStopped = false;
        }
    }
}


