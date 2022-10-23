using System.Collections;
using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using TMPro;
using Google.Play.Review;
public class GameManager : MonoBehaviour
{
    public bool gameOver = false;
    public bool gameStart = false;
    public bool realGame = true;
    public float gameStartFloat = 0;
    public GameObject buttonManager;
    public GameObject Sun;
    public GameObject Asteroid;
    public GameObject startCanvas;
    public GameObject shopCanvas;

    public int starbies;
    public int starbiesAtGameStart;

    public float highscore;
    public float score = 0;
    private float referenceTime;
    public float scoreFactor = 0;
    public GameObject obstacleManagerObject;
    public GameObject newHighscoreText;

    public float minX, maxX, minY, maxY;

    public GameObject cam;

    public int spriteNumber;

    public AudioSource homeAudio;
    public AudioSource gameAudio;
    public AudioSource obstacleMoveAudio;
    public AudioSource obstacleInvalidAudio;
    public bool mute;

    public bool tutorial;
    public GameObject switchPanel;

    public bool connectedToGPS;
    public TMP_Text googlePlayButtonText;
    public static PlayGamesPlatform platform;

    private ReviewManager reviewManager;
    private PlayReviewInfo playReviewInfo;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("TimesLaunched", PlayerPrefs.GetInt("TimesLaunched", 0) + 1);

        if (PlayerPrefs.GetInt("DidTutorial", 0) < 1 )
        {
            buttonManager.GetComponent<AllButtons>().StartCoroutine(buttonManager.GetComponent<AllButtons>().SwitchToScene("Tutorial"));
            PlayerPrefs.SetInt("DidTutorial", 1);
        }

        if (PlayerPrefs.GetInt("TimesLaunched", 0) > 2)
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            SignInToGooglePlayServices();
        }

        StartCoroutine(VisibleGame());

        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        minX = bottomCorner.x;
        maxX = topCorner.x;
        minY = bottomCorner.y;
        maxY = topCorner.y;

        highscore = PlayerPrefs.GetFloat("highscore", 0);
        starbies = PlayerPrefs.GetInt("starbies", 0);

        homeAudio.Play();
        StartCoroutine(FadeAudioSource.FadeAudio(homeAudio, 4f, 0.5f));

        if (PlayerPrefs.GetInt("TimesLaunched", 0) % 10 == 0 || PlayerPrefs.GetInt("TimesLaunched", 0) % 15 == 0)
        {
            StartCoroutine(RequestReviews());
        }

        if (PlayerPrefs.GetInt("TimesLaunched", 0) % 25 == 0 && PlayerPrefs.GetInt("Reviewed", 0) < 1)
        {
            buttonManager.GetComponent<AllButtons>().OpenReview();
        }
    }

    IEnumerator RequestReviews()
    {
        reviewManager = new ReviewManager();

        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(requestFlowOperation.Error.ToString());
            yield break;
        }
        playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(launchFlowOperation.Error.ToString());
            yield break;
        }
    }

    public void SignInToGooglePlayServices()
    {
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
        {
            switch (result)
            {
                case SignInStatus.Success:
                    connectedToGPS = true;
                    googlePlayButtonText.text = "SIGN-OUT OF GOOGLE PLAY";
                    break;
                default:
                    connectedToGPS = false;
                    googlePlayButtonText.text = "SIGN-IN TO GOOGLE PLAY";
                    break;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        gameStart = buttonManager.GetComponent<AllButtons>().gameStart;

        if (gameStart == true && gameStartFloat < 1 && realGame == true)
        {
            gameStartFloat += 1;
            StartCoroutine(StartGame());
            scoreFactor = 1;
            referenceTime = Time.realtimeSinceStartup;
        }

        if (scoreFactor != 0 && obstacleManagerObject.GetComponent<ObstacleManager>().speed > 0)
        {
            score += (Time.realtimeSinceStartup - referenceTime) * scoreFactor * 0.65f;
        }

        if (score > PlayerPrefs.GetFloat("highscore", 0) && gameOver == true)
        {
            PlayerPrefs.SetFloat("highscore", score);
            highscore = score;
            newHighscoreText.SetActive(true);

            if (connectedToGPS)
            {
                Debug.Log("Reporting Leaderboard Score");
                long leaderboardScore = Convert.ToInt64(PlayerPrefs.GetFloat("highscore, 0"));
                Social.ReportScore(leaderboardScore, GPGSIds.leaderboard_best_score, (success) =>
                {
                    if (!success) Debug.LogError("Unable to post highscore");
                });
            }
            else
            {
                Debug.Log("Not Signed in... unable to report score");
            }
            
        }

        if (mute)
        {
            homeAudio.mute = true;
            gameAudio.mute = true;
            obstacleMoveAudio.mute = true;
            obstacleInvalidAudio.mute = true;
        }

        if (!mute)
        {
            homeAudio.mute = false;
            gameAudio.mute = false;
            obstacleMoveAudio.mute = false;
            obstacleInvalidAudio.mute = false;
        }
    }

    private void SendGamesPlayedProgress()
    {
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_astro_starter, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_astro_learner, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_astro_novice, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_astro_gamer, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_astro_veteran, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_astronomical, 1, (success) => { });
    }
    
    IEnumerator StartGame()
    {
        PlayerPrefs.SetInt("TimesPlayed", PlayerPrefs.GetInt("TimesPlayed", 0) + 1);
        SendGamesPlayedProgress();

        shopCanvas.SetActive(false);
        starbiesAtGameStart = PlayerPrefs.GetInt("starbies", 0);

        StartCoroutine(Shake(1f, 0.1f));
        while (startCanvas.GetComponent<CanvasGroup>().alpha != 0)
        {
            startCanvas.GetComponent<CanvasGroup>().alpha -= 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        startCanvas.SetActive(false);

        StartCoroutine(FadeAudioSource.FadeAudio(homeAudio, 5f, 0f));
        gameAudio.Play();
        StartCoroutine(FadeAudioSource.FadeAudio(gameAudio, 5f, 0.6f));

        Sun.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 70f * Time.deltaTime);
        gameObject.GetComponent<AsteroidSpriteChanger>().ChangeSprite(PlayerPrefs.GetInt("SpriteNumberInUse", 0));
        Asteroid.GetComponent<AsteroidRotation>().enabled = true;
        yield return new WaitForSeconds(2f);
        obstacleManagerObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        Destroy(Sun);
        homeAudio.Stop();
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = cam.transform.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        cam.transform.localPosition = originalPos;
    }
    IEnumerator VisibleGame()
    {
        while (switchPanel.GetComponent<CanvasGroup>().alpha > 0)
        {
            switchPanel.GetComponent<CanvasGroup>().alpha -= 0.04f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        switchPanel.SetActive(false);
        yield return null;
    }
}

public static class FadeAudioSource
{
    public static IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}


