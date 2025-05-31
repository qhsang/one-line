using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the GameObject around its Y axis at 45 degrees per second
        transform.Rotate(0, 0, -45 * Time.deltaTime);
    }
}
