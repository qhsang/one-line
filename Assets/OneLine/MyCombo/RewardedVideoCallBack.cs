//using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedVideoCallBack : MonoBehaviour {

    private void Start()
    {
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
       // if (AdmobController.instance.rewardBasedVideo != null)
       // {
       //     AdmobController.instance.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
       // }
#endif
    }

    private const string ACTION_NAME = "rewarded_video";
   // public void HandleRewardBasedVideoRewarded(object sender, Reward args)
   // {
   //     Toast.instance.ShowMessage("You've received a free hint", 2.5f);
    //    PlayerData.instance.NumberOfHints += 1;
    //    PlayerData.instance.SaveData();

    //    var controller = FindObjectOfType<UIControllerForGame>();
    //    if (controller != null) controller.UpdateHint();

  //      CUtils.SetActionTime(ACTION_NAME);
    //}

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
      //  if (AdmobController.instance.rewardBasedVideo != null)
      //  {
      //      AdmobController.instance.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
      //  }
#endif
    }
}
