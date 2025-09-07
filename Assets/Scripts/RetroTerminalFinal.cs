using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RetroTerminalFinal : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI terminalText;

    [Header("Typing Settings")]
    public float letterDelay = 0.03f;
    public int maxLinesVisible = 20;

    [Header("Sound")]
    public AudioClip typingSound;
    public AudioClip terminalShutdownSound;
    public AudioClip backgroundMusic;

    private List<string> linesBuffer = new List<string>();
    private string currentInput = "";
    private bool showCursor = true;
    private bool isTyping = false;
    private Color originalColor;
    private AudioSource audioSource;

    [Header("079 Lines")]
    [TextArea] public string[] lines079;

    [Header("Credits")]
    [TextArea] public string[] creditsLines;
    public float delayBetweenCredits = 1f;
    public float delayAfterCredits = 5f;

    [Header("Scene")]
    public string menuSceneName = "MainMenu";

    private void Start()
    {
        terminalText.text = "";
        originalColor = terminalText.color;

        audioSource = gameObject.AddComponent<AudioSource>();

        StartCoroutine(BlinkCursor());
        StartCoroutine(RunFinale());
    }

    private void Update()
    {
        // Effet flicker CRT quand pas en train de taper
        if (!isTyping && Random.value < 0.01f)
        {
            terminalText.color = originalColor * Random.Range(0.85f, 1f);
            StartCoroutine(ResetColorAfterDelay(0.08f));
        }
    }

    IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        terminalText.color = originalColor;
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            showCursor = !showCursor;
            RefreshText();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void RefreshText()
    {
        string bufferText = string.Join("\n", linesBuffer.ToArray());
        string inputLine = "C:\\> " + currentInput;
        if (showCursor) inputLine += "|";
        terminalText.text = bufferText + "\n" + inputLine;
    }

    private IEnumerator RunFinale()
    {
        // --- Répliques 079 ---
        foreach (string line in lines079)
        {
            yield return StartCoroutine(TypeLineColored("079> " + line, Color.red, TextAlignmentOptions.TopLeft));
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(2f);

        // --- Shutdown terminal ---
        if (terminalShutdownSound != null)
            audioSource.PlayOneShot(terminalShutdownSound);

        yield return new WaitForSeconds(1f);

        linesBuffer.Clear();
        currentInput = "";
        terminalText.text = "";

        // --- Crédits (style terminal, centrés) ---
        if (backgroundMusic != null)
        {
            AudioSource musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        foreach (string credit in creditsLines)
        {
            yield return StartCoroutine(TypeLineColored("C:\\> " + credit, originalColor, TextAlignmentOptions.Center));
            yield return new WaitForSeconds(delayBetweenCredits);
        }

        yield return new WaitForSeconds(delayAfterCredits);

        // Retour menu
        SceneManager.LoadScene(menuSceneName);
    }

    private IEnumerator TypeLineColored(string line, Color color, TextAlignmentOptions alignment)
    {
        isTyping = true;

        if (typingSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        string currentLine = "";
        foreach (char c in line)
        {
            currentLine += c;
            string coloredLine = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{currentLine}</color>";
            RefreshTextBuffer(coloredLine, alignment);
            yield return new WaitForSeconds(letterDelay);
        }

        linesBuffer.Add($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{currentLine}</color>");
        if (linesBuffer.Count > maxLinesVisible)
            linesBuffer.RemoveAt(0);

        if (audioSource.isPlaying)
            audioSource.Stop();

        isTyping = false;
    }

    private void RefreshTextBuffer(string currentLine, TextAlignmentOptions alignment)
    {
        string bufferText = string.Join("\n", linesBuffer.ToArray());
        string inputLine = currentLine;
        if (showCursor) inputLine += "|";

        terminalText.alignment = alignment;
        terminalText.text = bufferText + "\n" + inputLine;
    }
}



