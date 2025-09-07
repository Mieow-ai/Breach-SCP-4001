using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementWithFreelook : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 90f;
    public float alignSpeed = 5f;

    [Header("Camera")]
    public Transform cameraTransform;
    public Camera playerCam;

    [Header("Breath Sounds")]
    public AudioClip calmBreathClip;
    public AudioClip heavyBreathClip;
    public AudioClip recoverBreathClip;

    // Breath
    private AudioSource calmBreathSource;
    private AudioSource heavyBreathSource;
    private AudioSource recoverBreathSource;

    private CharacterController controller;
    private float verticalLook = 0f;
    private float horizontalLook = 0f;
    private bool isSprinting = false;
    private bool wasSprintingLastFrame = false;
    private float recoverDelay = 2f;
    private float recoverTimer = 0f;
    private bool isRecovering = false;

    // ----------- FORCED LOOK -----------
    [Header("Forced Look Settings")]
    public bool isForcedLook = false;
    public Transform lookTarget;
    public float focusDuration = 2.5f;
    public float focusZoom = 35f;

    private float defaultFOV;
    private bool isFocusing = false;

    // --- NEW: Player dead flag ---
    private bool isDead = false;

    void Start()
    {
        mouseSensitivity = GameSettings.Instance.mouseSensitivity;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Breath setup
        calmBreathSource = gameObject.AddComponent<AudioSource>();
        calmBreathSource.loop = true;
        calmBreathSource.playOnAwake = false;
        calmBreathSource.clip = calmBreathClip;

        heavyBreathSource = gameObject.AddComponent<AudioSource>();
        heavyBreathSource.loop = true;
        heavyBreathSource.playOnAwake = false;
        heavyBreathSource.clip = heavyBreathClip;

        recoverBreathSource = gameObject.AddComponent<AudioSource>();
        recoverBreathSource.loop = false;
        recoverBreathSource.playOnAwake = false;
        recoverBreathSource.clip = recoverBreathClip;

        PlayCalmBreath();

        // FOV
        if (playerCam != null)
            defaultFOV = playerCam.fieldOfView;
    }

    void Update()
    {
        if (isDead)
            return; // Bloque tous les inputs si mort

        if (!isForcedLook)
            HandleMouseLook();
        else
            ForceLookAtTarget();

        HandleMovement();
        HandleBreathingSounds();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        verticalLook -= mouseY;
        verticalLook = Mathf.Clamp(verticalLook, -90f, 90f);

        horizontalLook += mouseX;
        horizontalLook = Mathf.Clamp(horizontalLook, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(verticalLook, horizontalLook, 0f);
    }

    void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        bool wasSprintingLastFrameCopy = isSprinting; // copie car on met à jour après

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        if (Mathf.Abs(moveZ) > 0.01f)
        {
            float targetYRotation = transform.eulerAngles.y + horizontalLook;
            Quaternion targetRotation = Quaternion.Euler(0f, targetYRotation, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, alignSpeed * Time.deltaTime);
            horizontalLook = Mathf.Lerp(horizontalLook, 0f, alignSpeed * Time.deltaTime);
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.SimpleMove(move * speed);

        wasSprintingLastFrame = wasSprintingLastFrameCopy;
    }

    void HandleBreathingSounds()
    {
        if (isSprinting && !wasSprintingLastFrame)
        {
            PlayHeavyBreath();
            isRecovering = false;
            recoverTimer = 0f;
        }
        else if (!isSprinting && wasSprintingLastFrame)
        {
            isRecovering = true;
            recoverTimer = 0f;
        }

        if (isRecovering)
        {
            recoverTimer += Time.deltaTime;
            if (recoverTimer >= recoverDelay)
            {
                PlayRecoverBreath();
                isRecovering = false;
            }
        }

        if (!isSprinting && !isRecovering && !calmBreathSource.isPlaying)
        {
            PlayCalmBreath();
        }
    }

    void PlayCalmBreath()
    {
        if (calmBreathClip == null) return;
        StopAllBreaths();
        calmBreathSource.Play();
    }

    void PlayHeavyBreath()
    {
        if (heavyBreathClip == null) return;
        StopAllBreaths();
        heavyBreathSource.Play();
    }

    void PlayRecoverBreath()
    {
        if (recoverBreathClip == null) return;
        StopAllBreaths();
        recoverBreathSource.Play();
    }

    void StopAllBreaths()
    {
        calmBreathSource.Stop();
        heavyBreathSource.Stop();
        recoverBreathSource.Stop();
    }

    // --------- Forced Look Logic ---------
    void ForceLookAtTarget()
    {
        if (lookTarget == null || cameraTransform == null) return;

        Vector3 direction = lookTarget.position - cameraTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime * 3f);

        if (playerCam != null)
        {
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, focusZoom, Time.deltaTime * 2f);
        }
    }

    public void StartLookAt(Transform target)
    {
        lookTarget = target;
        isForcedLook = true;
        isFocusing = true;
        StartCoroutine(ResetLookAfterDelay());
    }

    IEnumerator ResetLookAfterDelay()
    {
        yield return new WaitForSeconds(focusDuration);
        isForcedLook = false;
        isFocusing = false;

        if (playerCam != null)
            playerCam.fieldOfView = defaultFOV;
    }

    // --- NEW: appelée quand joueur meurt ---
    public void Die()
    {
        isDead = true;
        if (controller != null)
            controller.enabled = false; // désactive collisions & mouvement
    }

    public void StopLookAt()
    {
        isForcedLook = false;
        if (playerCam != null)
            playerCam.fieldOfView = defaultFOV;
    }
}
