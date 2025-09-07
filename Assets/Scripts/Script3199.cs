using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public enum MonsterState { Chase, Flee, AggressiveChase, Attack, PostAttack }
    public MonsterState currentState;

    [Header("Références")]
    public Transform player;
    public Transform chaseTarget;
    public Camera playerCamera;
    public Animator animator;
    public NavMeshAgent agent;
    public MonsterAudioManager audioManager;

    [Header("Paramètres de comportement")]
    public float aggressiveDistance = 5f;
    public float lookDetectionRange = 25f;
    [Range(0, 180)] public float lookThresholdAngle = 45f;

    [Header("Vitesses")]
    public float normalSpeed = 3.5f;
    public float aggressiveSpeed = 7f;

    [Header("Fuite")]
    public float fleeRunDistance = 10f;

    [Header("Screamer")]
    public GameObject screamerUI;
    public RawImage screamerImage;
    public VideoPlayer screamerVideo;
    public string gameOverSceneName = "GameOver";
    private bool hasAttacked = false;

    private Vector3 fleeStartPosition;
    private bool fleeing = false;
    public bool canMove = false;
    private bool chaseAllowed = false;

    void Start()
    {
        if (GameSettings.Instance.introDone)
        {
            AllowChase();
        }
        else
        {
            chaseAllowed = false;
            agent.isStopped = true;
        }

        currentState = MonsterState.Chase;

        if (playerCamera == null)
            playerCamera = Camera.main;

        agent.acceleration = 25f;
        agent.angularSpeed = 720f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = 30;
        agent.autoBraking = false;

        InitializeScreamer();
    }

    void Update()
    {
        if (!chaseAllowed || hasAttacked) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isLookedAt = IsPlayerLookingAtMe();

        switch (currentState)
        {
            case MonsterState.Chase:
                agent.speed = normalSpeed;
                agent.isStopped = false;
                animator.SetBool("isRunning", false);
                agent.SetDestination(chaseTarget != null ? chaseTarget.position : player.position);

                audioManager.PlayAmbiance();
                audioManager.PlayPasLents();

                if (isLookedAt && distance <= lookDetectionRange && distance > aggressiveDistance)
                {
                    StartFlee();
                }
                else if (distance <= aggressiveDistance)
                {
                    currentState = MonsterState.AggressiveChase;
                }
                break;

            case MonsterState.Flee:
                if (!fleeing) StartFlee();
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    fleeing = false;
                    currentState = MonsterState.Chase;

                    agent.speed = normalSpeed;
                    animator.SetBool("isRunning", false);
                    audioManager.StopPas();
                    audioManager.PlayPasLents();
                }
                break;

            case MonsterState.AggressiveChase:
                agent.speed = aggressiveSpeed;
                agent.isStopped = false;
                animator.SetBool("isRunning", true);
                agent.SetDestination(player.position);
                audioManager.PlayPasRapides();
                break;
        }

        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 lookDir = agent.velocity.normalized;
            lookDir.y = 0;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 10f
            );
        }
    }

    private void StartFlee()
    {
        Vector3 directionAway = (transform.position - player.position).normalized;
        Vector3 rawTarget = transform.position + directionAway * fleeRunDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rawTarget, out hit, fleeRunDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Vector3 randomDir = (directionAway + Random.insideUnitSphere * 0.5f).normalized;
            Vector3 altTarget = transform.position + randomDir * fleeRunDistance;
            if (NavMesh.SamplePosition(altTarget, out hit, fleeRunDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        agent.speed = aggressiveSpeed;
        agent.isStopped = false;
        animator.SetBool("isRunning", true);

        audioManager.StopPas();
        audioManager.PlayPasRapides();

        fleeing = true;
        fleeStartPosition = transform.position;
        currentState = MonsterState.Flee;
    }

    private bool IsPlayerLookingAtMe()
    {
        if (playerCamera == null) return false;

        Vector3 directionToMonster = (transform.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToMonster);
        float dist = Vector3.Distance(playerCamera.transform.position, transform.position);

        return angle < lookThresholdAngle && dist <= lookDetectionRange;
    }

    // -------------------------
    // Screamer
    // -------------------------
    private void InitializeScreamer()
    {
        if (screamerVideo != null)
        {
            screamerVideo.loopPointReached += OnScreamerVideoEnd;
            screamerVideo.playOnAwake = false;
            screamerVideo.Prepare(); // Prépare à l’avance
        }

        if (screamerImage != null)
        {
            Color c = screamerImage.color;
            c.a = 0f;
            screamerImage.color = c;
            screamerImage.texture = Texture2D.blackTexture;

            if (screamerVideo != null && screamerVideo.targetTexture != null)
            {
                RenderTexture rt = screamerVideo.targetTexture;
                RenderTexture activeRT = RenderTexture.active;
                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);
                RenderTexture.active = activeRT;
            }
        }

        if (screamerUI != null)
            screamerUI.SetActive(true);
    }

    private void PlayScreamer()
    {
        if (screamerUI == null || screamerVideo == null || screamerImage == null) return;

        Color c = screamerImage.color;
        c.a = 1f;
        screamerImage.color = c;

        screamerImage.texture = screamerVideo.targetTexture;

        if (!screamerVideo.isPrepared)
        {
            screamerVideo.prepareCompleted += PlayPreparedScreamer;
            screamerVideo.Prepare();
        }
        else
        {
            PlayPreparedScreamer(screamerVideo);
        }
    }

    private void PlayPreparedScreamer(VideoPlayer vp)
    {
        screamerVideo.prepareCompleted -= PlayPreparedScreamer;
        screamerVideo.Stop();
        screamerVideo.Play();
    }

    private void OnScreamerVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(GoToGameOver());
    }

    private IEnumerator GoToGameOver()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(gameOverSceneName);
    }

    // -------------------------
    // Attaque sur collision
    // -------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (hasAttacked) return;

        if (other.CompareTag("Player") && currentState == MonsterState.AggressiveChase)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;

            animator.SetTrigger("attack");
            audioManager.StopPas();
            audioManager.PlayCriAttaque();

            hasAttacked = true;
            PlayScreamer();
            currentState = MonsterState.PostAttack;
        }
    }

    public void AllowChase()
    {
        chaseAllowed = true;
        currentState = MonsterState.Chase;
        agent.isStopped = false;
    }
}













