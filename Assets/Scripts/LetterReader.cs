using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterReader : MonoBehaviour
{
    public static LetterReader Instance { get; private set; }

    [Header("UI")]
    public GameObject blurPanel;
    public Image letterImage;
    public TextMeshProUGUI readPromptText;
    public KeyCode readKey = KeyCode.T;

    private Sprite currentLetterSprite;
    private bool isReading = false;
    private bool promptShown = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (blurPanel) blurPanel.SetActive(false);
        if (letterImage) letterImage.gameObject.SetActive(false);
        if (readPromptText) readPromptText.gameObject.SetActive(false);
    }

    public void SetLetterSprite(Sprite sprite)
    {
        currentLetterSprite = sprite;
    }

    public void ShowReadPromptOnce()
    {
        if (!promptShown && readPromptText)
        {
            readPromptText.text = "Appuyez sur T pour lire la feuille";
            readPromptText.gameObject.SetActive(true);
            promptShown = true;
        }
    }

    void Update()
    {
        if (currentLetterSprite == null) return;

        if (Input.GetKeyDown(readKey))
        {
            isReading = !isReading;
            
            if (blurPanel) blurPanel.SetActive(isReading);
            if (letterImage)
            {
                letterImage.sprite = currentLetterSprite;
                letterImage.gameObject.SetActive(isReading);
            }
            if (readPromptText && isReading) readPromptText.gameObject.SetActive(false);
        }
    }
}