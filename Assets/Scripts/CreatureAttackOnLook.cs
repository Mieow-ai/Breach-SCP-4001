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

    [Tooltip("Temps pendant lequel la caméra du joueur est fixée sur 096")]
    public float phase2FreezeDuration = 3f; 
    [Tooltip("Temps TOTAL de la crise avant que 096 ne commence à courir. Baisse cette valeur si tu trouves qu'il est trop long !")]
    public float phase2TotalDuration = 12f; 

    private float pathUpdateTimer = 0f;

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
        agent.updateRotation = false; // Désactivé en Idle pour éviter des tremblements
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.radius = 0.5f; 
        agent.height = 3f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        if (playerCamera != null)
            playerScript = playerCamera.GetComponentInParent<PlayerMovementWithFreelook>();
    }

    private void Update()
    {
        if (currentState == State.Charging)
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                // On ne recalcule la destination que toutes les 0.1 secondes pour optimiser l'IA
                pathUpdateTimer += Time.deltaTime;
                if (pathUpdateTimer >= 0.1f)
                {
                    agent.SetDestination(playerCamera.position);
                    pathUpdateTimer = 0f;
                }
            }
            return; 
        }

        if (currentState == State.Idle)
        {
            if (IsPlayerLooking())
            {
                StartCoroutine(TriggeredSequence());
            }
        }
    }

    bool IsPlayerLooking()
    {
        if (playerCamera == null || faceCollider == null) return false;

        Vector3 directionToFace = (faceCollider.transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, directionToFace);
        float distance = Vector3.Distance(playerCamera.position, faceCollider.transform.position);

        if (angle > viewAngleThreshold || distance > maxDetectionDistance) return false;

        // On avance un peu l'origine pour ne pas taper le joueur lui-même
        Vector3 rayOrigin = playerCamera.position + (playerCamera.forward * 0.2f);
        
        // DEBUG : Affiche une ligne rouge dans la fenêtre "Scene" d'Unity pour voir où va le rayon
        Debug.DrawRay(rayOrigin, directionToFace * maxDetectionDistance, Color.red);

        RaycastHit hit;
        
        // On tire vers directionToFace, et on force la détection des Triggers
        if (Physics.Raycast(rayOrigin, directionToFace, out hit, maxDetectionDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            if (hit.collider == faceCollider || hit.collider.transform.root == this.transform.root)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator TriggeredSequence()
    {
        currentState = State.Angry; // Verrouille l'état

        // Phase 2 : On fige 096
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        
        animator.SetBool("IsAngry", true); // Hurlement

        // Forcer le joueur à le regarder
        if (playerScript) playerScript.StartLookAt(faceTarget != null ? faceTarget : transform);

        var flashlight = playerCamera.GetComponentInParent<FlashlightToggle>();
        if (flashlight)
        {
            flashlight.ForceTurnOffAndLock();
            if (flashlightUIController) flashlightUIController.HideUI();
        }

        if (rageLightsController) rageLightsController.SetRageState(true);
        if (ambianceManager) ambianceManager.PlayRageAmbience();

        // On attend que le joueur soit "débloqué"
        yield return new WaitForSeconds(phase2FreezeDuration);
        if (playerScript) playerScript.StopLookAt();

        // On attend la fin du cri
        yield return new WaitForSeconds(Mathf.Max(0, phase2TotalDuration - phase2FreezeDuration));

        // Phase 3 : Libération de la bête
        animator.SetBool("IsAngry", false);
        animator.SetBool("IsCharging", true);
        if (rageLightsController) rageLightsController.SetRageState(false);

        // On lui laisse 0.5 sec pour commencer à bouger ses jambes avant d'avancer
        yield return new WaitForSeconds(0.5f);

        // On réactive la poursuite
        if (agent.isActiveAndEnabled)
        {
            // Si le monstre a été légèrement surélevé, on force son "raccrochage" au NavMesh
            agent.Warp(transform.position); 

            agent.isStopped = false;
            agent.speed = chargeSpeed;
            
            // --- ROTATION AUTOMATIQUE DU NAVMESH ---
            agent.updateRotation = true; // Laisse Unity gérer la rotation pour qu'elle soit fluide
            agent.angularSpeed = 800f;   // Vitesse de pivot ultra réactive
            
            agent.acceleration = 25f;    // Accélération naturelle
            agent.autoBraking = false;   // Ne ralentit jamais en t'approchant
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }

        if (attackScript) attackScript.canAttack = true;

        if (flashlight)
        {
            flashlight.Unlock();
            if (flashlightUIController) flashlightUIController.ShowAvailableUI();
        }

        // On passe en Charging ! Le Update() va maintenant prendre le relai
        currentState = State.Charging; 
    }

    public void StopMovement()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    public void ResumeMovement()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh && currentState == State.Idle)
        {
            agent.speed = normalSpeed;
            agent.isStopped = false;
        }
    }
}


