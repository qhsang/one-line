using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoChangeColorText : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = Color.yellow;
    [SerializeField] private float transitionSpeed = 1f;
    
    [Header("Transition Trigger")]
    [SerializeField] private bool useGameObjectCondition = false;
    [SerializeField] private GameObject conditionObject;
    [SerializeField] private bool customCondition = false;

    private Text textComponent;
    private TextMeshProUGUI tmpComponent;
    private bool hasTextComponent = false;
    private bool hasTMPComponent = false;
    
    private Color targetColor;
    private Color currentColor;

    void Start()
    {
        // Check for Text component
        textComponent = GetComponent<Text>();
        if (textComponent != null)
        {
            hasTextComponent = true;
            currentColor = textComponent.color;
        }
        
        // Check for TextMeshProUGUI component
        tmpComponent = GetComponent<TextMeshProUGUI>();
        if (tmpComponent != null)
        {
            hasTMPComponent = true;
            currentColor = tmpComponent.color;
        }
        
        if (!hasTextComponent && !hasTMPComponent)
        {
            Debug.LogWarning("No Text or TextMeshProUGUI component found on " + gameObject.name);
        }
    }

    void Update()
    {
        // Determine target color based on condition
        bool condition = customCondition;
        
        if (useGameObjectCondition && conditionObject != null)
        {
            condition = conditionObject.activeSelf;
        }
        
        targetColor = condition ? colorB : colorA;
        
        // Smoothly transition to target color
        currentColor = targetColor;
        
        // Apply color to the text component
        if (hasTextComponent)
        {
            textComponent.color = currentColor;
        }
        else if (hasTMPComponent)
        {
            tmpComponent.color = currentColor;
        }
    }
}