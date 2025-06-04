using UnityEngine;

public class AutoZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    [Tooltip("Minimum zoom scale")]
    public float minZoom = 0.8f;
    
    [Tooltip("Maximum zoom scale")]
    public float maxZoom = 1.2f;
    
    [Tooltip("Speed of the zoom cycle in seconds")]
    public float zoomSpeed = 2.0f;
    
    [Tooltip("Smoothness of the zoom transition")]
    [Range(0.1f, 10.0f)]
    public float smoothness = 1.0f;
    
    private Vector3 initialScale;
    private float zoomTimer = 0f;
    
    void Start()
    {
        // Store the initial scale of the object
        initialScale = transform.localScale;
    }
    
    void Update()
    {
        // Increment the timer
        zoomTimer += Time.deltaTime * zoomSpeed;
        
        // Calculate the zoom factor using a sine wave for smooth transitions
        float zoomFactor = Mathf.Lerp(minZoom, maxZoom, (Mathf.Sin(zoomTimer) + 1f) / 2f);
        
        // Apply the zoom to the object's scale
        transform.localScale = Vector3.Lerp(
            transform.localScale, 
            initialScale * zoomFactor, 
            Time.deltaTime * smoothness
        );
    }
}
