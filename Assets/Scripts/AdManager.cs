using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using TMPro;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    private string playStoreID = "**CONFIDENTIAL**";

    private string interstitialAd = "Interstitial_Android";
    public string rewardedAd = "Rewarded_Android";
    public string dailyRewardAd = "Daily_Reward";

    public bool isTestAd;

    public GameObject asteroidManager;
    public GameObject gameManagerObject;
    public Button respawnButton;

    public GameObject rewardBackground;

    public GameObject buttonManager;
    public GameObject oopsCanvas;
    public TMP_Text oopsReason;

    public GameObject doubleStarbieCanvas;
    public Button doubleStarbieButton;

    private void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(playStoreID, true);
    }

    public void PlayInterstitialAd()
    {
        if (!Advertisement.IsReady(interstitialAd))
        {
            return;
        }

        Advertisement.Show(interstitialAd);
    }

    public void PlayRewardedAd()
    {
        Advertisement.Show(rewardedAd);
    }

    public void PlayDailyRewardAd()
    {
        Advertisement.Show(dailyRewardAd);
    }

    public void OnUnityAdsReady(string placementId)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Ads Ready");
    }

    public void OnUnityAdsDidError(string message)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Ads NOT Ready");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        gameManagerObject.GetComponent<GameManager>().mute = true;
        Time.timeScale = 0;
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                Time.timeScale = 1;
                gameManagerObject.GetComponent<GameManager>().mute = false;
                Debug.Log("Ad Failed");
                break;

            case ShowResult.Skipped:
                Time.timeScale = 1;
                gameManagerObject.GetComponent<GameManager>().mute = false;
                Debug.Log("Ad Skipped");

                if (placementId == rewardedAd || placementId == dailyRewardAd)
                {
                    oopsReason.text = "WATCH THE WHOLE AD TO EARN YOUR REWARD";
                    buttonManager.GetComponent<AllButtons>().StartCoroutine(buttonManager.GetComponent<AllButtons>().SettingsAnimation(0.9f, oopsCanvas));
                }
                break;

            case ShowResult.Finished:
                Time.timeScale = 1;
                gameManagerObject.GetComponent<GameManager>().mute = false;

                if (placementId == rewardedAd)
                {
                    asteroidManager.GetComponent<AllCollisions>().StopAllCoroutines();
                    asteroidManager.GetComponent<AllCollisions>().StartCoroutine(asteroidManager.GetComponent<AllCollisions>().Respawn());
                    Debug.Log("Ad Success: Respawning");
                }

                if (placementId == dailyRewardAd)
                {
                    doubleStarbieCanvas.SetActive(true);
                    doubleStarbieButton.GetComponent<Button>().interactable = false;
                    Debug.Log("Ad Success: Doubling Starbies");
                }
                break;
        }
    }
}
