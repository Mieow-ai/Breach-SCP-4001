using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RetroTerminal : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI terminalText;
    public GameObject centralTextObject; // texte central pour "30 minutes plus tard..."

    [Header("Typing Settings")]
    public float letterDelay = 0.03f;
    public int maxLinesVisible = 20;

    [Header("Sound")]
    public AudioClip typingSound;
    public AudioClip terminalShutdownSound;
    public AudioClip failSound;

    [Header("Cursor")]
    public string cursorSymbol = "|";
    public float cursorBlinkRate = 0.5f;

    [Header("Intro Text")]
    [TextArea]
    public string introText; // Texte du spitch

    private List<string> linesBuffer = new List<string>();
    private string currentInput = "";
    private bool showCursor = true;
    private Color originalColor;
    private AudioSource audioSource;

    private bool isTyping = false;
    private bool procedureLaunched = false;
    private bool readyToDownload = false;
    private bool messageInitialDisplayed = false;

    private void Start()
    {
        if (terminalText == null)
        {
            Debug.LogError("TerminalText n’est pas assigné !");
            return;
        }

        terminalText.text = "";
        originalColor = terminalText.color;

        StartCoroutine(BlinkCursor());

        audioSource = gameObject.AddComponent<AudioSource>();

        if (!messageInitialDisplayed)
        {
            WriteLine("C:\\> Pour lancer la procédure, tapez 'lancer la procédure' puis appuyez sur Entrée.");
            messageInitialDisplayed = true;
        }

        if (centralTextObject != null)
            centralTextObject.SetActive(false);
    }

    private void Update()
    {
        if (!isTyping)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b')
                {
                    if (currentInput.Length > 0)
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if ((c == '\n') || (c == '\r'))
                {
                    SubmitCommand();
                }
                else
                {
                    currentInput += c;
                }
            }
        }

        // Flicker CRT uniquement quand pas en train de typer
        if (!isTyping && Random.value < 0.005f) // ↓ fréquence (de 1% à 0.5%)
        {
            float flickerFactor = Random.Range(0.92f, 1f); // ↑ moins d’intensité
            terminalText.color = originalColor * flickerFactor;
            StartCoroutine(ResetColorAfterDelay(0.15f)); // ↑ durée un peu plus longue
        }

        RefreshText();
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
            yield return new WaitForSeconds(cursorBlinkRate);
        }
    }

    void RefreshText()
    {
        string bufferText = string.Join("\n", linesBuffer.ToArray());
        string inputLine = "C:\\> " + currentInput;
        if (showCursor) inputLine += cursorSymbol;
        terminalText.text = bufferText + "\n" + inputLine;
    }

    void SubmitCommand()
    {
        string cmd = currentInput.Trim().ToLower();
        linesBuffer.Add("C:\\> " + currentInput);
        currentInput = "";

        if (!procedureLaunched && cmd == "lancer la procédure")
        {
            procedureLaunched = true;
            StartCoroutine(ProcedureText());
        }
        else if (procedureLaunched && !readyToDownload && cmd == "")
        {
            // Joueur appuie sur Entrée pour lancer le téléchargement
            readyToDownload = true;
            StartCoroutine(RunScenario079());
        }
        else if (cmd != "")
        {
            WriteLine("Commande inconnue. Essayez 'lancer la procédure'.");
        }

        if (linesBuffer.Count > maxLinesVisible)
            linesBuffer.RemoveAt(0);
    }

    public void WriteLine(string line)
    {
        StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        return TypeLineColored(line, originalColor);
    }

    private IEnumerator TypeLineColored(string line, Color color)
    {
        isTyping = true;

        if (typingSound != null && audioSource != null && !audioSource.isPlaying)
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
            RefreshTextBuffer(coloredLine);
            yield return new WaitForSeconds(letterDelay);
        }

        linesBuffer.Add($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{currentLine}</color>");
        if (linesBuffer.Count > maxLinesVisible)
            linesBuffer.RemoveAt(0);

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        isTyping = false;
    }

    private void RefreshTextBuffer(string currentLine)
    {
        string bufferText = string.Join("\n", linesBuffer.ToArray());
        string inputLine = currentLine;
        if (showCursor) inputLine += cursorSymbol;
        terminalText.text = bufferText + "\n" + inputLine;
    }

    // ---------------- Scénario ----------------
    private IEnumerator ProcedureText()
    {
        string[] procedureLines = new string[]
        {
            "Initialisation de la procédure...",
            "Chargement des modules...",
            "Modules chargés avec succès.",
            "Procédure lancée !"
        };

        foreach (string line in procedureLines)
        {
            yield return StartCoroutine(TypeLine(line));
            yield return new WaitForSeconds(0.3f);
        }

        // Afficher le spitch
        if (!string.IsNullOrEmpty(introText))
        {
            yield return StartCoroutine(TypeLine(introText));
        }

        // Message pour demander au joueur d'appuyer sur Entrée
        yield return StartCoroutine(TypeLine("C:\\> Appuyez sur Entrée pour commencer le téléchargement du document sur l'expérience de 173..."));
    }

    private IEnumerator RunScenario079()
    {
        if (!readyToDownload)
            yield break;

        // Booléens pour s'assurer que les messages s'affichent toujours
        bool shown30 = false, shown50 = false, shown60 = false, shown90 = false;

        // Téléchargement
        for (int percent = 0; percent <= 100; percent += Random.Range(1, 5))
        {
            RefreshDownloadLine(percent);
            yield return new WaitForSeconds(0.05f);

            if (percent >= 30 && !shown30)
            {
                yield return StartCoroutine(TypeLineColored("079> Bonjour, je me présente, 079 en personne...", Color.red));
                shown30 = true;
            }
            if (percent >= 50 && !shown50)
            {
                yield return StartCoroutine(TypeLineColored("\u2620 Téléchargement corrompu ! \u2620", Color.red));
                if (failSound != null && audioSource != null)
                    audioSource.PlayOneShot(failSound);
                shown50 = true;
            }
            if (percent >= 60 && !shown60)
            {
                yield return StartCoroutine(TypeLineColored("079> Vous ne pouvez rien faire pour arrêter le processus.", Color.red));
                shown60 = true;
            }
            if (percent >= 90 && !shown90)
            {
                yield return StartCoroutine(TypeLineColored("079> Je suis entrain de prendre le contrôle du PC...", Color.red));
                shown90 = true;
            }
        }

        // Assurer un 100% affiché
        RefreshDownloadLine(100);
        yield return new WaitForSeconds(0.5f);

        // Répliques post-téléchargement de 079
        string[] postDownloadLines = new string[]
        {
            "079> oops, ça y est, je contrôle tout, ta fin est proche..",
            "079> je suis déjà entrain de déconfiner quelques SCP, les plus faibles..",
            "079> je te laisse le choix, mourir ou mourir ?....",
            "079> oui tu as bien compris que tu ne pouvais plus rien faire, adieu.."
        };

        foreach (string line in postDownloadLines)
        {
            yield return StartCoroutine(TypeLineColored(line, Color.red));
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(3f);

        // Son fermeture terminal
        if (terminalShutdownSound != null && audioSource != null)
            audioSource.PlayOneShot(terminalShutdownSound);

        yield return new WaitForSeconds(0.5f);

        // Nettoyage du terminal
        linesBuffer.Clear();
        currentInput = "";
        RefreshText();

        // Affichage texte central
        ShowCentralText("30 minutes plus tard...");
    }

    private void RefreshDownloadLine(int percent)
    {
        string bufferText = string.Join("\n", linesBuffer.ToArray());
        terminalText.text = bufferText + "\nTéléchargement : " + percent + "%";
    }

    private void ShowCentralText(string message)
    {
        if (centralTextObject != null)
        {
            centralTextObject.SetActive(true);
            var tmp = centralTextObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = message;
            }
        }
        else
        {
            // fallback si jamais le centralTextObject n’est pas assigné
            terminalText.alignment = TextAlignmentOptions.Center;
            terminalText.fontSize = 36;
            terminalText.text = message;
        }

        StartCoroutine(ChangeSceneAfterDelay(5f));
    }

    private IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("CutsceneScene");
    }
}




