﻿//using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedVideoButton : MonoBehaviour
{
    private const string ACTION_NAME = "rewarded_video";

    private void Start()
    {
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
     //   if (AdmobController.instance.rewardBasedVideo != null)
      //  {
      //      AdmobController.instance.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
     //   }
#endif
    }

    public void OnClick()
    {
       // if (IsAvailableToShow())
       // {
       //     AdmobController.instance.ShowRewardBasedVideo();
       // }
     if (!IsActionAvailable())
        {
            int remainTime = (int)(GameConfig.instance.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
            Toast.instance.ShowMessage("Please wait " + remainTime + " seconds for the next ad");
        }
        else
        {
            Toast.instance.ShowMessage("Ad is not available at the moment");
        }

        Sound.instance.PlayButton();
    }

//    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
//    {
//
//    }

//    public bool IsAvailableToShow()
 //   {
//        return IsActionAvailable() && IsAdAvailable();
 //   }

    private bool IsActionAvailable()
    {
        return CUtils.IsActionAvailable(ACTION_NAME, GameConfig.instance.rewardedVideoPeriod);
    }

//    private bool IsAdAvailable()
 //   {
      //  if (AdmobController.instance.rewardBasedVideo == null) return false;
      //  bool isLoaded = AdmobController.instance.rewardBasedVideo.IsLoaded();
      //  return isLoaded;
 //   }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
     //   if (AdmobController.instance.rewardBasedVideo != null)
       // {
      //      AdmobController.instance.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
       // }
#endif
    }
}
