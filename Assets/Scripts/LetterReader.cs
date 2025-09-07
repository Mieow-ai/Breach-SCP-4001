using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterReader : MonoBehaviour
{
    public static LetterReader Instance;

    [Header("UI")]
    public GameObject blurPanel;        // Panel arrière-plan
    public Image letterImage;           // Image de la lettre
    public TextMeshProUGUI readPromptText;
    public KeyCode readKey = KeyCode.T;

    private Sprite currentLetterSprite;
    private bool isReading = false;
    private bool promptShown = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

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
            if (!isReading)
            {
                if (readPromptText) readPromptText.gameObject.SetActive(false);
                if (blurPanel) blurPanel.SetActive(true);
                if (letterImage)
                {
                    letterImage.sprite = currentLetterSprite;
                    letterImage.gameObject.SetActive(true);
                }
                isReading = true;
            }
            else
            {
                if (blurPanel) blurPanel.SetActive(false);
                if (letterImage) letterImage.gameObject.SetActive(false);
                isReading = false;
            }
        }
    }
}