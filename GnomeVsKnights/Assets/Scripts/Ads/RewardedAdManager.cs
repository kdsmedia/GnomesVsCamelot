using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

/// <summary>
/// Singleton managing AdMob rewarded ads (Google Mobile Ads Unity plugin v10 API).
/// Initialize once at app launch via <see cref="Initialize"/>. Preload an ad, then
/// call <see cref="ShowRewardedAd"/> with a reward callback.
/// </summary>
public class RewardedAdManager : MonoBehaviour
{
    public static RewardedAdManager Instance { get; private set; }

    [Header("AdMob Configuration")]
    [Tooltip("AdMob Rewarded Ad Unit ID for Android.")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-6881903056221433/1098989475";

    [Tooltip("AdMob Rewarded Ad Unit ID for iOS (unused on Android build).")]
    [SerializeField] private string iosAdUnitId = "unused";

    [Tooltip("Use Google's official test ad unit in Editor / development builds.")]
    [SerializeField] private bool useTestAdsInDevelopment = true;

    // Official Google rewarded test ad unit ID.
    private const string TestRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

    private RewardedAd _rewardedAd;
    private bool _isInitializing;
    private bool _isInitialized;

    // Pending request: the reward action to invoke once an ad completes.
    private Action<bool> _pendingRewardCallback;
    private bool _isLoadingAd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>True once MobileAds.Initialize has completed.</summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>Initialize the Google Mobile Ads SDK. Call once at app launch.</summary>
    public void Initialize()
    {
        if (_isInitialized || _isInitializing)
        {
            return;
        }

        _isInitializing = true;

        MobileAds.Initialize((InitializationStatus status) =>
        {
            _isInitializing = false;
            _isInitialized = true;
            Debug.Log("[AdMob] MobileAds SDK initialized.");
            LoadRewardedAd();
        });
    }

    private string GetAdUnitId()
    {
#if UNITY_EDITOR
        if (useTestAdsInDevelopment)
        {
            return TestRewardedAdUnitId;
        }
#elif DEVELOPMENT_BUILD
        if (useTestAdsInDevelopment)
        {
            return TestRewardedAdUnitId;
        }
#endif
        return Application.platform == RuntimePlatform.IPhonePlayer
            ? iosAdUnitId
            : androidAdUnitId;
    }

    /// <summary>Preload a rewarded ad so it is ready to show.</summary>
    public void LoadRewardedAd()
    {
        string adUnitId = GetAdUnitId();

        // Clean up the previous ad before creating a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("[AdMob] Loading rewarded ad...");
        _isLoadingAd = true;

        var adRequest = new AdRequest();
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            _isLoadingAd = false;

            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdMob] Rewarded ad failed to load: {error?.GetMessage()}");
                _rewardedAd = null;
                return;
            }

            _rewardedAd = ad;
            RegisterEventHandlers(ad);
            Debug.Log("[AdMob] Rewarded ad loaded successfully.");
        });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"[AdMob] Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("[AdMob] Rewarded ad recorded an impression.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("[AdMob] Rewarded ad was clicked.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("[AdMob] Rewarded ad full screen content opened.");
        };

        // Reload so the next show request has an ad ready.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdMob] Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogWarning($"[AdMob] Rewarded ad failed to open full screen content: {error.GetMessage()}");
            LoadRewardedAd();
        };
    }

    /// <summary>Returns true if a rewarded ad is loaded and ready to display.</summary>
    public bool IsRewardedAdReady()
    {
        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }

    /// <summary>
    /// Show the rewarded ad. The callback receives true if the player earned the
    /// reward (watched the ad to completion), false otherwise.
    /// </summary>
    public void ShowRewardedAd(Action<bool> onRewardEarned)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _pendingRewardCallback = onRewardEarned;

            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"[AdMob] Reward earned: {reward.Amount} {reward.Type}");
                _pendingRewardCallback?.Invoke(true);
                _pendingRewardCallback = null;
            });
        }
        else
        {
            Debug.Log("[AdMob] Rewarded ad is not ready yet. Reloading...");
            onRewardEarned?.Invoke(false);

            if (!_isLoadingAd)
            {
                LoadRewardedAd();
            }
        }
    }
}
