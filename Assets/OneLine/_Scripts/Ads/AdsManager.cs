using System;
using UnityEngine;
using Unity.Services.LevelPlay;
using System.Threading.Tasks;

/// <summary>
/// Enum for banner positions
/// </summary>
public enum BannerPos
{
    TopCenter,
    BottomCenter,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
}

/// <summary>
/// Configuration class for LevelPlay ads
/// Contains app key and ad unit IDs
/// </summary>
[System.Serializable]
public class AdsConfiguration
{
    public string AppKey { get; private set; }
    public string RewardedAdUnitId { get; private set; }
    public string InterstitialAdUnitId { get; private set; }
    public string BannerAdUnitId { get; private set; }
    public BannerPos BannerPosition { get; private set; } = BannerPos.BottomCenter;
    
    // Track which ad types are enabled
    public bool HasRewardedAds => !string.IsNullOrEmpty(RewardedAdUnitId);
    public bool HasInterstitialAds => !string.IsNullOrEmpty(InterstitialAdUnitId);
    public bool HasBannerAds => !string.IsNullOrEmpty(BannerAdUnitId);
    
    // Private constructor to enforce use of builder
    private AdsConfiguration() { }
    
    /// <summary>
    /// Builder class for creating AdsConfiguration objects
    /// </summary>
    public class Builder
    {
        private readonly AdsConfiguration _config = new AdsConfiguration();
        
        /// <summary>
        /// Sets the app key (required)
        /// </summary>
        public Builder WithAppKey(string appKey)
        {
            _config.AppKey = appKey;
            return this;
        }
        
        /// <summary>
        /// Adds rewarded ad support
        /// </summary>
        public Builder WithRewardedAds(string unitId)
        {
            _config.RewardedAdUnitId = unitId;
            return this;
        }
        
        /// <summary>
        /// Adds interstitial ad support
        /// </summary>
        public Builder WithInterstitialAds(string unitId)
        {
            _config.InterstitialAdUnitId = unitId;
            return this;
        }
        
        /// <summary>
        /// Adds banner ad support
        /// </summary>
        public Builder WithBannerAds(string unitId)
        {
            _config.BannerAdUnitId = unitId;
            return this;
        }
        
        /// <summary>
        /// Sets the position for banner ads
        /// </summary>
        /// <param name="position">Banner position (default is BottomCenter if not specified)</param>
        public Builder WithBannerPosition(BannerPos position)
        {
            _config.BannerPosition = position;
            return this;
        }
        
        /// <summary>
        /// Builds and returns the configuration
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when AppKey is not set</exception>
        public AdsConfiguration Build()
        {
            if (string.IsNullOrEmpty(_config.AppKey))
            {
                throw new ArgumentException("App key is required");
            }
            
            return _config;
        }
    }
}

/// <summary>
/// AdsManager - A simplified utility class to manage LevelPlay ads.
/// Provides only essential methods for interacting with ads.
/// </summary>
public static class AdsManager
{
    #region Configuration
    private static AdsConfiguration adsConfig;
    private static bool testMode = true;
    private static float interstitialCooldown = 60f;
    private static bool adsInitialized = false;
    private static bool adsEnabled = false;
    #endregion

    #region Ad References
    private static LevelPlayRewardedAd rewardedAd;
    private static LevelPlayInterstitialAd interstitialAd;
    private static LevelPlayBannerAd bannerAd;
    #endregion

    #region State Variables
    private static float lastInterstitialTime = -1000f;
    private static bool isBannerShowing = false;
    private static bool isBannerLoaded = false;
    private static MonoBehaviour coroutineRunner;
    #endregion

    #region Events
    // Reward Events
    public static event Action<LevelPlayReward> OnRewardGranted;
    public static event Action OnRewardedAdClosed;
    
    // Interstitial Events
    public static event Action OnInterstitialClosed;
    #endregion

    /// <summary>
    /// Convert BannerPos enum to the LevelPlayBannerPosition type required by the library
    /// </summary>
    private static com.unity3d.mediation.LevelPlayBannerPosition GetLevelPlayBannerPosition(BannerPos position)
    {
        switch (position)
        {
            case BannerPos.TopLeft:
                return com.unity3d.mediation.LevelPlayBannerPosition.TopLeft;
            case BannerPos.TopCenter:
                return com.unity3d.mediation.LevelPlayBannerPosition.TopCenter;
            case BannerPos.TopRight:
                return com.unity3d.mediation.LevelPlayBannerPosition.TopRight;
            case BannerPos.Center:
                return com.unity3d.mediation.LevelPlayBannerPosition.Center;
            case BannerPos.BottomLeft:
                return com.unity3d.mediation.LevelPlayBannerPosition.BottomLeft;
            case BannerPos.BottomRight:
                return com.unity3d.mediation.LevelPlayBannerPosition.BottomRight;
            case BannerPos.BottomCenter:
            default:
                return com.unity3d.mediation.LevelPlayBannerPosition.BottomCenter;
        }
    }

    /// <summary>
    /// Initialize the SDK and ad units with required configuration
    /// </summary>
    /// <param name="config">The ad configuration containing app key and ad unit IDs</param>
    public static void Initialize(AdsConfiguration config)
    {
        if (adsInitialized) 
        {
            Debug.Log("[AdsManager] SDK already initialized, skipping initialization");
            return;
        }

        adsConfig = config;

        Debug.Log("[AdsManager] Initializing SDK with AppKey: " + adsConfig.AppKey);
        
        // Create a hidden GameObject to run coroutines if needed
        if (coroutineRunner == null)
        {
            GameObject go = new GameObject("AdsManagerCoroutineRunner");
            coroutineRunner = go.AddComponent<CoroutineHelper>();
            GameObject.DontDestroyOnLoad(go);
            Debug.Log("[AdsManager] Created CoroutineRunner GameObject");
        }
        
        // Register initialization callbacks
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        
        // Initialize the SDK
        Debug.Log("[AdsManager] Calling LevelPlay.Init...");
        LevelPlay.Init(adsConfig.AppKey);
    }

    /// <summary>
    /// Initialize all ad units after SDK initialization
    /// </summary>
    private static void EnableAds()
    {
        if (!adsInitialized)
        {
            Debug.Log("[AdsManager] Cannot enable ads: SDK not initialized");
            return;
        }
        
        if (!adsEnabled)
        {
            Debug.Log("[AdsManager] Cannot enable ads: ads are disabled");
            return;
        }
        
        Debug.Log("[AdsManager] Starting to load configured ad units...");
        
        if (adsConfig.HasRewardedAds)
            LoadRewardedAd();
        else
            Debug.Log("[AdsManager] Rewarded ads not configured");
            
        if (adsConfig.HasInterstitialAds)
            LoadInterstitialAd();
        else
            Debug.Log("[AdsManager] Interstitial ads not configured");
            
        if (adsConfig.HasBannerAds)
            LoadBannerAd();
        else
            Debug.Log("[AdsManager] Banner ads not configured");
        
        Debug.Log("[AdsManager] All configured ad units initialization requested successfully");
    }

    /// <summary>
    /// Load all configured ad types
    /// </summary>
    public static void LoadAds()
    {
        if (!adsEnabled || !adsInitialized) return;
        
        if (adsConfig.HasRewardedAds)
            LoadRewardedAd();
        
        if (adsConfig.HasInterstitialAds)
            LoadInterstitialAd();
        
        if (adsConfig.HasBannerAds)
            LoadBannerAd();
    }

    #region Banner Methods
    
    /// <summary>
    /// Load banner ad
    /// </summary>
    private static void LoadBannerAd()
    {
        if (!adsConfig.HasBannerAds)
        {
            Debug.LogWarning("[AdsManager] Cannot load banner ad: No banner ad unit ID configured");
            return;
        }
        
        if (bannerAd != null)
        {
            bannerAd.DestroyAd();
        }
        
        // Convert our custom enum position to the library's position type
        com.unity3d.mediation.LevelPlayBannerPosition levelPlayPosition = GetLevelPlayBannerPosition(adsConfig.BannerPosition);
        
        // Create banner ad with converted position from configuration
        bannerAd = new LevelPlayBannerAd(
            adsConfig.BannerAdUnitId,
            position: levelPlayPosition
        );
        
        isBannerLoaded = false;
        
        // Register load event handler to support delayed showing
        bannerAd.OnAdLoaded += (adInfo) => {
            Debug.Log("[AdsManager] Banner Ad Loaded");
            isBannerLoaded = true;
            if (isBannerShowing)
            {
                Debug.Log("[AdsManager] Showing Banner that was queued");
                bannerAd.ShowAd();
            }
        };
        
        bannerAd.LoadAd();
    }

    /// <summary>
    /// Show banner ad at the position set in configuration (default is bottom center)
    /// If the banner is not loaded yet, it will be shown automatically once loaded
    /// </summary>
    public static void ShowBanner()
    {
        if (!adsEnabled || !adsInitialized) return;
        
        if (!adsConfig.HasBannerAds)
        {
            Debug.LogWarning("[AdsManager] Cannot show banner ad: No banner ad unit ID configured");
            return;
        }

        if (bannerAd == null)
        {
            Debug.Log("[AdsManager] Banner ad not initialized, loading now");
            LoadBannerAd();
        }

        Debug.Log("[AdsManager] Setting banner to show when ready");
        isBannerShowing = true;
        
        // If banner is already loaded, show it immediately
        if (bannerAd != null && isBannerLoaded)
        {
            Debug.Log("[AdsManager] Banner ad ready, showing immediately");
            bannerAd.ShowAd();
        }
        else
        {
            Debug.Log("[AdsManager] Banner ad not ready yet, will show when loaded");
        }
    }

    /// <summary>
    /// Hide the current banner ad
    /// </summary>
    public static void HideBanner()
    {
        if (bannerAd == null || !isBannerShowing) return;

        Debug.Log("[AdsManager] Hiding Banner");
        bannerAd.HideAd();
        isBannerShowing = false;
    }
    #endregion

    #region Interstitial Methods
    
    /// <summary>
    /// Load interstitial ad
    /// </summary>
    private static void LoadInterstitialAd()
    {
        if (!adsConfig.HasInterstitialAds)
        {
            Debug.LogWarning("[AdsManager] Cannot load interstitial ad: No interstitial ad unit ID configured");
            return;
        }
        
        if (interstitialAd != null)
        {
            interstitialAd.DestroyAd();
        }
        
        interstitialAd = new LevelPlayInterstitialAd(adsConfig.InterstitialAdUnitId);
        
        // Register close event handler
        interstitialAd.OnAdClosed += (adInfo) => {
            Debug.Log("[AdsManager] Interstitial Ad Closed");
            OnInterstitialClosed?.Invoke();
            interstitialAd.LoadAd();
        };
        
        interstitialAd.LoadAd();
    }

    /// <summary>
    /// Show interstitial ad if ready and not in cooldown period
    /// </summary>
    /// <returns>True if ad was shown, false otherwise</returns>
    public static bool ShowInterstitial()
    {
        if (!adsEnabled || !adsInitialized) return false;
        
        if (!adsConfig.HasInterstitialAds)
        {
            Debug.LogWarning("[AdsManager] Cannot show interstitial ad: No interstitial ad unit ID configured");
            return false;
        }
        
        // Check cooldown
        if (Time.time - lastInterstitialTime < interstitialCooldown)
        {
            Debug.Log("[AdsManager] Interstitial in cooldown period");
            return false;
        }

        // Check if ad is ready
        if (interstitialAd == null || !interstitialAd.IsAdReady())
        {
            Debug.Log("[AdsManager] Interstitial not ready, loading new one");
            LoadInterstitialAd();
            return false;
        }

        // Show ad
        Debug.Log("[AdsManager] Showing Interstitial");
        interstitialAd.ShowAd();
        lastInterstitialTime = Time.time;
        return true;
    }
    #endregion

    #region Rewarded Methods
    
    /// <summary>
    /// Load rewarded ad
    /// </summary>
    private static void LoadRewardedAd()
    {
        if (!adsConfig.HasRewardedAds)
        {
            Debug.LogWarning("[AdsManager] Cannot load rewarded ad: No rewarded ad unit ID configured");
            return;
        }
        
        if (rewardedAd != null)
        {
            rewardedAd.DestroyAd();
        }
        
        rewardedAd = new LevelPlayRewardedAd(adsConfig.RewardedAdUnitId);
        
        // Register reward and close event handlers
        rewardedAd.OnAdRewarded += (adInfo, reward) => {
            Debug.Log($"[AdsManager] Rewarded Ad Completed - Reward: {reward}");
            OnRewardGranted?.Invoke(reward);
            _rewardTcs?.TrySetResult(true);
        };
        
        rewardedAd.OnAdClosed += (adInfo) => {
            Debug.Log("[AdsManager] Rewarded Ad Closed");
            OnRewardedAdClosed?.Invoke();
            _rewardTcs?.TrySetResult(false);
            rewardedAd.LoadAd();
        };
        
        rewardedAd.LoadAd();
    }

    /// <summary>
    /// Check if a rewarded ad is loaded and ready to show
    /// </summary>
    /// <returns>True if a rewarded ad is ready to show, false otherwise</returns>
    public static bool IsRewardedAdLoaded()
    {
        if (!adsEnabled || !adsInitialized || !adsConfig.HasRewardedAds) return false;
        
        return rewardedAd != null && rewardedAd.IsAdReady();
    }

    // TaskCompletionSource to track reward completion
    private static TaskCompletionSource<bool> _rewardTcs;

    /// <summary>
    /// Show rewarded ad if ready and wait for result
    /// </summary>
    /// <returns>Task that completes with true if user earned the reward, false otherwise</returns>
    public static async Task<bool> ShowRewardedAd()
    {
        if (!adsEnabled || !adsInitialized) return false;
        
        if (!adsConfig.HasRewardedAds)
        {
            return false;
        }

        if (rewardedAd == null || !rewardedAd.IsAdReady())
        {
            LoadRewardedAd();
            return false;
        }

        // Create new TaskCompletionSource for this ad viewing
        _rewardTcs = new TaskCompletionSource<bool>();

        rewardedAd.ShowAd();
        
        // Wait for ad completion with 60-second timeout
        try {
            return await Task.WhenAny(_rewardTcs.Task, Task.Delay(100000)) == _rewardTcs.Task && 
                   await _rewardTcs.Task;
        }
        catch (Exception ex) {
            Debug.LogError($"[AdsManager] Error showing rewarded ad: {ex.Message}");
            return false;
        }
    }
    #endregion

    #region Init Callbacks
    
    private static void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log("[AdsManager] SDK Initialization Completed");
        adsInitialized = true;
        adsEnabled = true;
        EnableAds();
    }

    private static void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log($"[AdsManager] SDK Initialization Failed: {error}");
    }
    #endregion
}

/// <summary>
/// Helper class to run coroutines since static classes cannot
/// </summary>
public class CoroutineHelper : MonoBehaviour { }
