using UnityEngine;

public class FootSounds : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    public AudioSource audioSource;

    public float WalkStepInterval = 0.5f;
    public float RunStepInterval = 0.3f;
    private float currentStep;
    private float stepTimer;

    private bool IsRunning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStep = WalkStepInterval;
    }

    public void setRunning(bool running)
    {
        IsRunning = running;
        currentStep = running ? RunStepInterval : WalkStepInterval;
    }

    // Update is called once per frame
    void Update()
    {
        setRunning(CharacterIsRunning());
        PlayFootStepSounds();
    }

    private void PlayFootStepSounds()
    {
        if (CharacterIsMoving())
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= currentStep)
            {
                int randomIndex = Random.Range(0, footstepSounds.Length);
                audioSource.PlayOneShot(footstepSounds[randomIndex]);
                stepTimer = 0;

            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    private bool CharacterIsMoving()
    {
        return Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
    }

    private bool CharacterIsRunning()
    {
        return Input.GetAxis("Fire3") != 0;
    }
}
