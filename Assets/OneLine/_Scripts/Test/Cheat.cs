using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

public class Cheat : MonoBehaviour
{
    [SerializeField] private int requiredClicks = 20;
    [SerializeField] private float maxTimeBetweenClicks = 1f;
    [SerializeField] private GameObject cheatObj;
    private int clickCount = 0;
    private bool cheatsEnabled = false;
    private float _lastClickedTime = 0;
    
    public void RegisterClick()
    {
        clickCount++;
        _lastClickedTime = Time.time;
        // Check if we've reached the required number of clicks
        if (clickCount >= requiredClicks)
        {
            ActivateCheat();
            clickCount = 0;
        }
    }

    private void Update()
    {
        if(clickCount <= 0) 
            return;
        
        if (Time.time - _lastClickedTime >= maxTimeBetweenClicks)
        {
            clickCount = 0;
        }
    }

    private void ActivateCheat()
    {
        if (!cheatsEnabled)
        {
            cheatsEnabled = true;
            Debug.Log("Cheat code activated!");
            
            CheatActivated();
        }
    }
    
    private void CheatActivated()
    {
        cheatObj.SetActive(true);
    }
}
