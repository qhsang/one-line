using System;
using UnityEngine;
using UnityEngine.Advertisements;
 
public class AdsInitializer : MonoBehaviour
{
    private RewardedAdsButton _rewardedAdsButton;
 
    void Awake()
    {
        var configAds = new AdsConfiguration.Builder()
            .WithAppKey("228c8447d")
            .WithRewardedAds("oavpjynaaq7741cy")
            .Build();
        
        AdsManager.Initialize(configAds);
        AdsManager.LoadAds();
    }
}