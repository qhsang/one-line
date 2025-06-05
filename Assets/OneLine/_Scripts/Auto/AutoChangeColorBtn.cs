using UnityEngine;
using UnityEngine.UI;

public class AutoChangeColorBtn : MonoBehaviour
{
    [Header("Color Settings")]
    public Color colorA = Color.gray;
    public Color colorB = Color.white;
    
    [Header("References")]
    public GameObject targetObject;
    
    private Image image;
    private CanvasGroup canvasGroup;
    private bool lastTargetState = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the Image component on this GameObject
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("No Image component found on " + gameObject.name);
        }
        
        // Get the CanvasGroup component or add one if it doesn't exist
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Set initial color and alpha based on target state
        if (targetObject != null)
        {
            lastTargetState = targetObject.activeSelf;
            UpdateVisuals(lastTargetState);
        }
        else
        {
            // Default to first color if no target is set
            if (image != null)
            {
                image.color = colorA;
            }
            canvasGroup.alpha = 1.0f;
        }
    }

    // Instead of checking every frame, we'll check only when needed
    void OnEnable()
    {
        // Update visuals when this object becomes enabled
        if (targetObject != null)
        {
            UpdateVisuals(targetObject.activeSelf);
        }
    }
    
    void Update()
    {
        // Only check for changes, not every frame
        if (targetObject != null && lastTargetState != targetObject.activeSelf)
        {
            lastTargetState = targetObject.activeSelf;
            UpdateVisuals(lastTargetState);
        }
    }
    
    // Centralizes the logic for updating color and alpha
    private void UpdateVisuals(bool isTargetActive)
    {
        // Update color
        if (image != null)
        {
            image.color = isTargetActive ? colorA : colorB;
        }
        
        // Update alpha
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isTargetActive ? 0.5f : 1f;
        }
    }
}
