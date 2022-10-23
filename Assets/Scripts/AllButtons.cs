using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.Advertisements;
using UnityEngine.SocialPlatforms;

public class AllButtons : MonoBehaviour
{
    [HideInInspector]
    public bool gameStart = false;

    public float transitionSpeed;

    public GameObject startCanvasObject;
    public GameObject shopCanvasObject;
    public GameObject dummyCanvasObject;

    private RectTransform startCanvas;
    private RectTransform shopCanvas;
    public GameObject safeArea;

    public Vector3 startCanvasOrignalPos;
    public Vector3 shopCanvasOriginalPos;
    public Vector3 dummyCanvasOriginalPos;

    public GameObject gameManager;
    public GameObject asteroidManager;
    public TMP_Text starbiesText;

    public AudioSource storeSFX;
    public AudioSource startButtonsSFX;
    public AudioSource buySFX;
    public AudioSource equipSFX;
    public AudioSource replaySFX;
    public AudioSource invalidSFX;

    public GameObject settingsCanvas;
    public GameObject giftCanvas;
    public GameObject reviewCanvas;

    public GameObject switchPanel;
    public GameObject fadeButton;

    public GameObject monetizationManager;
    public GameObject oopsCanvas;
    public TMP_Text oopsReason;

    public GameObject doubleStarbieCanvas;

    private string internetCheck = "http://leatonm.net/wp-content/uploads/2017/candlepin/getdate.php";
    public TMP_Text googlePlayButtonText;

    public GameObject consentCanvas;
    public GameObject tutorialManager;


    private void Start()
    {
        startCanvas = startCanvasObject.GetComponent<RectTransform>();
        shopCanvas = shopCanvasObject.GetComponent<RectTransform>();

        startCanvasOrignalPos = startCanvas.localPosition;
        shopCanvasOriginalPos = shopCanvas.localPosition;
        dummyCanvasOriginalPos = dummyCanvasObject.GetComponent<RectTransform>().localPosition;

        Destroy(dummyCanvasObject);
    }

    public void ReplayButton()
    {
        StartCoroutine(ReplayButtonCR());
    }
    
    public void AdRespawnClick()
    {
        StartCoroutine(AdRespawnClickCR());
    }

    IEnumerator AdRespawnClickCR()
    {
        Debug.Log("connecting to internet");
        WWW www = new WWW(internetCheck);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Error: Not Connected to Internet");
            oopsReason.text = "WE COULD NOT DETECT AN INTERNET CONNECTION";
            StartCoroutine(SettingsAnimation(0.9f, oopsCanvas));
        }
        else
        {
            monetizationManager.GetComponent<AdManager>().PlayRewardedAd();
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void DoubleStarbieButtonClick()
    {
        StartCoroutine(DoubleStarbieButtonClickCR());
    }

    IEnumerator DoubleStarbieButtonClickCR()
    {
        Debug.Log("connecting to internet");
        WWW www = new WWW(internetCheck);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Error: Not Connected to Internet");
            oopsReason.text = "WE COULD NOT DETECT AN INTERNET CONNECTION";
            StartCoroutine(SettingsAnimation(0.9f, oopsCanvas));
        }
        else
        {
            monetizationManager.GetComponent<AdManager>().PlayDailyRewardAd();
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void DoubleStarbiesButton()
    {
        gameManager.GetComponent<GameManager>().starbies += asteroidManager.GetComponent<AllCollisions>().starbiesEarnedThisRound;
        PlayerPrefs.SetInt("starbies", gameManager.GetComponent<GameManager>().starbies);
        doubleStarbieCanvas.SetActive(false);
        StartCoroutine(SwitchToScene("Game"));
    }

    public void TutorialButton()
    {
        StartCoroutine(SwitchToScene("Tutorial"));
        startButtonsSFX.Play();
    }

    public void AchievementButton()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) =>
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        gameManager.GetComponent<GameManager>().connectedToGPS = true;
                        googlePlayButtonText.text = "SIGN-OUT OF GOOGLE PLAY";
                        Social.ShowAchievementsUI();
                        break;
                    default:
                        gameManager.GetComponent<GameManager>().connectedToGPS = false;
                        googlePlayButtonText.text = "SIGN-IN TO GOOGLE PLAY";
                        break;
                }
            });
        }
        else
        {
            Social.ShowAchievementsUI();
        }
    }

    public void LeaderboardButton()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) =>
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        gameManager.GetComponent<GameManager>().connectedToGPS = true;
                        googlePlayButtonText.text = "SIGN-OUT OF GOOGLE PLAY";
                        Social.ShowLeaderboardUI();
                        break;
                    default:
                        gameManager.GetComponent<GameManager>().connectedToGPS = false;
                        googlePlayButtonText.text = "SIGN-IN TO GOOGLE PLAY";
                        break;
                }
            });
        }
        else
        {
            Social.ShowLeaderboardUI();
        }
    }
    public void StartButton()
    {
        gameStart = true;
    }

    public void HomeToShop()
    {
        gameObject.GetComponent<StoreToHome>().enabled = false;
        gameObject.GetComponent<HomeToStore>().enabled = true;
        safeArea.SetActive(false);
        storeSFX.Play();
    }

    public void ShopToHome()
    {
        gameObject.GetComponent<HomeToStore>().enabled = false;
        gameObject.GetComponent<StoreToHome>().enabled = true;
        StartCoroutine(Wait(0.4f));
        storeSFX.Play();
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        safeArea.SetActive(true);
        yield return null;
    }

    public void CometRespawn()
    {
        if (gameManager.GetComponent<GameManager>().starbies >= 250)
        {
            gameManager.GetComponent<GameManager>().starbies -= 250;
            PlayerPrefs.SetInt("starbies", gameManager.GetComponent<GameManager>().starbies);
            asteroidManager.GetComponent<AllCollisions>().StopAllCoroutines();
            asteroidManager.GetComponent<AllCollisions>().StartCoroutine(asteroidManager.GetComponent<AllCollisions>().Respawn());
        }
        else
        {
            StartCoroutine(CannotBuy());
        }
    }
    IEnumerator CannotBuy()
    {
        Color color = starbiesText.color;
        int x = 0;
        invalidSFX.Play();

        while (x != 3)
        {
            starbiesText.CrossFadeColor(Color.red, 0.1f, true, false);
            yield return new WaitForSeconds(0.1f);
            starbiesText.CrossFadeColor(color, 0.1f, true, false);
            yield return new WaitForSeconds(0.1f);
            x += 1;
        }
        yield return null;
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
        StartCoroutine(SettingsAnimation(0.9f, settingsCanvas));
        startButtonsSFX.Play();
    }

    public void CloseSettings()
    {
        StartCoroutine(SettingsAnimation(0, settingsCanvas));
        startButtonsSFX.Play();
    }
    public void OpenGift()
    {
        giftCanvas.SetActive(true);
        StartCoroutine(SettingsAnimation(0.9f, giftCanvas));
        startButtonsSFX.Play();
    }

    public void CloseGift()
    {
        StartCoroutine(SettingsAnimation(0, giftCanvas));
        startButtonsSFX.Play();
    }

    public void OpenReview()
    {
        reviewCanvas.SetActive(true);
        StartCoroutine(SettingsAnimation(0.9f, reviewCanvas));
        startButtonsSFX.Play();
    }
    public void CloseReview()
    {
        StartCoroutine(SettingsAnimation(0, reviewCanvas));
        startButtonsSFX.Play();
    }

    public void CloseOopsCanvas()
    {
        StartCoroutine(SettingsAnimation(0, oopsCanvas));
        startButtonsSFX.Play();
    }

    public IEnumerator SettingsAnimation(float scale, GameObject settingsCanvas)
    {
        if (settingsCanvas.GetComponent<RectTransform>().localScale.x < scale)
        {
            while (settingsCanvas.GetComponent<RectTransform>().localScale.x < scale)
            {
                settingsCanvas.GetComponent<RectTransform>().localScale += new Vector3(0.08f, 0.08f, 0.08f);
                yield return new WaitForSecondsRealtime(0.005f);
            }
            yield return null;
        }
        else
        {
            while (settingsCanvas.GetComponent<RectTransform>().localScale.x > scale)
            {
                settingsCanvas.GetComponent<RectTransform>().localScale -= new Vector3(0.08f, 0.08f, 0.08f);
                yield return new WaitForSecondsRealtime(0.005f);
            }

            settingsCanvas.SetActive(false);
            yield return null;
        }

        yield return null;
    }

    public void BoxCatURL()
    {
        Application.OpenURL("https://freemusicarchive.org/music/BoxCat_Games#");
    }

    public void ExplosionSoundURL()
    {
        Application.OpenURL("https://opengameart.org/content/boom-pack-1");
    }

    public void PrivacyPolicyURL()
    {
        Application.OpenURL("https://unity3d.com/legal/privacy-policy");
    }

    public void ReviewButton()
    {
        PlayerPrefs.SetInt("Reviewed", 1);
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public void GooglePlayButton()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) =>
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        gameManager.GetComponent<GameManager>().connectedToGPS = true;
                        googlePlayButtonText.text = "SIGN-OUT OF GOOGLE PLAY";
                        break;
                    default:
                        gameManager.GetComponent<GameManager>().connectedToGPS = false;
                        googlePlayButtonText.text = "SIGN-IN TO GOOGLE PLAY";
                        break;
                }
            });
        }
        else
        {
            PlayGamesPlatform.Instance.SignOut();
            gameManager.GetComponent<GameManager>().connectedToGPS = false;
            googlePlayButtonText.text = "SIGN-IN TO GOOGLE PLAY";
        }
    }

    public void backButtonTutorial()
    {
        StartCoroutine(BackFromTutorial());
    }

    public IEnumerator SwitchToScene(string scene)
    {
        switchPanel.SetActive(true);
        while (switchPanel.GetComponent<CanvasGroup>().alpha < 1)
        {
            switchPanel.GetComponent<CanvasGroup>().alpha += 0.04f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        Advertisement.RemoveListener(monetizationManager.GetComponent<AdManager>());
        SceneManager.LoadScene(sceneName: scene);
    }

    public IEnumerator BackFromTutorial()
    {
        fadeButton.SetActive(true);
        while (fadeButton.GetComponent<CanvasGroup>().alpha < 1)
        {
            fadeButton.GetComponent<CanvasGroup>().alpha += 0.04f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        SceneManager.LoadScene(sceneName: "Game");
    }

    public IEnumerator ReplayButtonCR()
    {
        replaySFX.Play();

        if (PlayerPrefs.GetInt("TimesPlayed", 0) % 3 == 0)
        {
            Debug.Log("connecting to internet");
            WWW www = new WWW(internetCheck);
            yield return www;
            if (www.error != null)
            {
                Debug.Log("Error: Not Connected to Internet");
            }
            else
            {
                monetizationManager.GetComponent<AdManager>().PlayInterstitialAd();
                yield return new WaitForSeconds(0.2f);
            }         
        }
        StartCoroutine(SwitchToScene("Game"));
        yield return null;
    }

    public void YesConsent()
    {
        StartCoroutine(SettingsAnimation(0, consentCanvas));
        startButtonsSFX.Play();
        tutorialManager.GetComponent<Tutorial>().StartCoroutine(tutorialManager.GetComponent<Tutorial>().TutorialStart());
        PlayerPrefs.SetInt("DidTutorial", 2);
    }

    public void NoConsent()
    {
        PlayerPrefs.SetInt("DidTutorial", 0);
        PlayerPrefs.SetInt("TimesLaunched", 0);
        startButtonsSFX.Play();
        Application.Quit();
    }

    public void ConsentHelp()
    {
        startButtonsSFX.Play();
        Application.OpenURL("https://support.google.com/accounts/answer/1350409#zippy=");
    }
}
