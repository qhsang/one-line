using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RandomText : MonoBehaviour
{
    private List<string> textOptions = new List<string>
    {
        "NICE!",
        "AWESOME!",
        "GREAT JOB!",
        "BRILLIANT!",
        "PERFECT!",
        "EXCELLENT!",
        "WELL DONE!",
        "AMAZING!",
        "FANTASTIC!",
        "SUPERB!",
        "WONDERFUL!",
        "YOU DID IT!",
        "SUCCESS!",
        "VICTORY!",
        "GENIUS!",
        "SHARP MIND!",
        "INCREDIBLE!",
        "SMART MOVE!",
        "BRAVO!",
        "NICELY SOLVED!"
    };

    [Tooltip("Use TextMeshPro instead of regular Text component")]
    [SerializeField] private bool useTextMeshPro = false;

    private Text uiText;
    private TextMeshPro textMeshPro;
    private TextMeshProUGUI textMeshProUGUI;
    
    
    private void Awake()
    {
        // Find the appropriate text component
        if (useTextMeshPro)
        {
            textMeshPro = GetComponent<TextMeshPro>();
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            
            if (textMeshPro == null && textMeshProUGUI == null)
            {
                Debug.LogError("No TextMeshPro component found on this GameObject. Please add one or set useTextMeshPro to false.");
            }
        }
        else
        {
            uiText = GetComponent<Text>();
            
            if (uiText == null)
            {
                Debug.LogError("No Text component found on this GameObject. Please add one or set useTextMeshPro to true.");
            }
        }
    }

    private void OnEnable()
    {
        // Set a random text when the component is enabled
        SetRandomText();
    }

    /// <summary>
    /// Sets a random text from the list to the text component
    /// </summary>
    public void SetRandomText()
    {
        if (textOptions == null || textOptions.Count == 0)
        {
            Debug.LogWarning("Text options list is empty. Please add some text options in the inspector.");
            return;
        }

        string randomText = GetRandomText();
        UpdateTextComponent(randomText);
    }

    /// <summary>
    /// Gets a random string from the list
    /// </summary>
    private string GetRandomText()
    {
        int randomIndex = Random.Range(0, textOptions.Count);
        return textOptions[randomIndex];
    }

    /// <summary>
    /// Updates the text component with the given string
    /// </summary>
    private void UpdateTextComponent(string text)
    {
        if (useTextMeshPro)
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = text;
            }
            else if (textMeshProUGUI != null)
            {
                textMeshProUGUI.text = text;
            }
        }
        else if (uiText != null)
        {
            uiText.text = text;
        }
    }
}
