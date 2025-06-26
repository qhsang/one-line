using System;
using System.Threading.Tasks;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
 
public class RewardedAdsButton : MonoBehaviour{
  [SerializeField] Button _showAdButton;
  public Text balanceText;
  

  private void Awake()
  {
    _showAdButton.onClick.AddListener(ShowAd);
  }

  private async void OnEnable()
  {
    // Disable button until ad is ready
    _showAdButton.interactable = false;
    
    // Wait for ads to initialize, but stay on main thread
    while (!AdsManager.IsRewardedAdLoaded())
    {
      await Task.Delay(100);
    }
    
    AdsManager.OnRewardGranted += OnUnityAdsShowComplete;
    AdsManager.OnRewardedAdClosed += () =>
    {
      _showAdButton.interactable = true;
    };
    _showAdButton.interactable = true;
  }
  
  public async void ShowAd()
  {
    _showAdButton.interactable = false;
    await AdsManager.ShowRewardedAd();
  }
 
  // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
  public void OnUnityAdsShowComplete(LevelPlayReward levelPlayReward)
  {
      PlayerData.instance.NumberOfHints ++;
      PlayerData.instance.SaveData();
      UpdateBalance();
      _showAdButton.interactable = true;
  }
  
  private void UpdateBalance()
  {
    if(!balanceText)
    {
      return;
    }
    balanceText.text = "" + PlayerData.instance.NumberOfHints;
  }
 
  void OnDestroy()
  {
    // Clean up the button listeners:
    AdsManager.OnRewardGranted -= OnUnityAdsShowComplete;
    _showAdButton.onClick.RemoveAllListeners();
  }
}
