using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject storyCanvasPanel;
    public TMP_Text storyText;
    public TMP_Text starbieText;
    public TMP_Text scoreText;

    public GameObject asteroidManager;
    public GameObject Asteroid;

    public TMP_Text generalHelpText;
    public GameObject helpPanelFlashLeft;
    public TMP_Text obstacleHelpTextLeft;
    public GameObject helpPanelFlashRight;
    public TMP_Text obstacleHelpTextRight;

    public GameObject gameManagerObject;
    public GameObject obstacleManagerObject;
    public GameObject stackOne;
    public GameObject stackTwo;
    public GameObject stackThree;
    public GameObject stackFour;

    public TMP_Text starbieHelpText;
    public GameObject Comet;
    public TMP_Text endText;
    public GameObject asteroidHolder;

    public GameObject buttonManager;
    public GameObject fadeButton;

    public GameObject backButton;
    public GameObject consentCanvas;

    // Start is called before the first frame update
    void Start()
    {
        buttonManager.GetComponent<AllButtons>().gameStart = true;
        gameManagerObject.GetComponent<GameManager>().realGame = false;
        gameManagerObject.GetComponent<AsteroidSpriteChanger>().ChangeSprite(0);

        if (PlayerPrefs.GetInt("DidTutorial", 0) > 1)
        {
            backButton.SetActive(true);
        }

        if(PlayerPrefs.GetInt("DidTutorial", 0) == 1)
        {
            consentCanvas.SetActive(true);
            buttonManager.GetComponent<AllButtons>().StartCoroutine(buttonManager.GetComponent<AllButtons>().SettingsAnimation(1, consentCanvas));
        }
        else
        {
            StartCoroutine(TutorialStart());
        }
    }

    IEnumerator FadeText(TMP_Text text, int targetAlpha, float fadeTime)
    {
        if (text.GetComponent<CanvasGroup>().alpha > targetAlpha)
        {
            while (text.GetComponent<CanvasGroup>().alpha > targetAlpha)
            {
                text.GetComponent<CanvasGroup>().alpha -= 0.02f;
                yield return new WaitForSecondsRealtime(fadeTime);
            }
            yield return null;
        }
        else
        {
            while (text.GetComponent<CanvasGroup>().alpha < 1f)
            {
                text.GetComponent<CanvasGroup>().alpha += 0.02f;
                yield return new WaitForSecondsRealtime(fadeTime);
            }
            yield return null;
        }

        yield return null;
    }

    IEnumerator ObstacleSpawn(GameObject stack, float distance, int position, int counterHolder)
    {
        stack.transform.position = new Vector3(distance, gameManagerObject.GetComponent<GameManager>().minY - 0.5f, transform.position.z);
        obstacleManagerObject.GetComponent<ObstacleManager>().speed = 1;
        obstacleManagerObject.GetComponent<ObstacleManager>().counter = 10;

        while (stack.transform.position.y < gameManagerObject.GetComponent<GameManager>().minY + 1.2f)
        {
            yield return null;
        }

        Time.timeScale = 0.2f;

        if (position == 0)
        {
            StartCoroutine(FadeText(obstacleHelpTextLeft, 1, 0.002f));
            while (helpPanelFlashLeft.GetComponent<CanvasGroup>().alpha < 0.2f)
            {
                helpPanelFlashLeft.GetComponent<CanvasGroup>().alpha += 0.01f;
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }
        else
        {
            StartCoroutine(FadeText(obstacleHelpTextRight, 1, 0.005f));
            while (helpPanelFlashRight.GetComponent<CanvasGroup>().alpha < 0.2f)
            {
                helpPanelFlashRight.GetComponent<CanvasGroup>().alpha += 0.01f;
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }

        obstacleManagerObject.GetComponent<ObstacleManager>().counter = counterHolder;

        while (stack.transform.position.x != 0)
        {
            yield return null;
            if (stack.transform.position.y > Asteroid.transform.position.y - 0.5)
            {
                stack.transform.position = new Vector3(0f, transform.position.y, transform.position.z);
            }
        }

        Time.timeScale = 1;
        obstacleManagerObject.GetComponent<ObstacleManager>().counter = 10;

        if (position == 0)
        {
            StartCoroutine(FadeText(obstacleHelpTextLeft, 0, 0.002f));
            while (helpPanelFlashLeft.GetComponent<CanvasGroup>().alpha > 0)
            {
                helpPanelFlashLeft.GetComponent<CanvasGroup>().alpha -= 0.01f;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
        else
        {
            StartCoroutine(FadeText(obstacleHelpTextRight, 0, 0.005f));
            while (helpPanelFlashRight.GetComponent<CanvasGroup>().alpha > 0)
            {
                helpPanelFlashRight.GetComponent<CanvasGroup>().alpha -= 0.01f;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }  
    }

    IEnumerator DestoryObstacle(GameObject stack)
    {
        while (stack.transform.position.y < gameManagerObject.GetComponent<GameManager>().maxY + 0.5)
        {
            yield return null;
        }

        Destroy(stack);
    }

    IEnumerator AsteroidRotation(float duration)
    {
        float timePassed = 0;

        while (timePassed < duration)
        {
            Asteroid.transform.Rotate(new Vector3(0f, 0f, Asteroid.GetComponent<AsteroidRotation>().speed) * Time.deltaTime);
            timePassed += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    public IEnumerator TutorialStart()
    {
        StartCoroutine(FadeText(scoreText, 1, 0.02f));
        StartCoroutine(FadeText(starbieText, 1, 0.02f));

        while (storyCanvasPanel.GetComponent<CanvasGroup>().alpha > 0)
        {
            storyCanvasPanel.GetComponent<CanvasGroup>().alpha -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }

        storyCanvasPanel.SetActive(false);
        storyText.enabled = false;

        asteroidManager.SetActive(true);
        Asteroid.GetComponent<AsteroidRotation>().enabled = true;

        yield return new WaitForSecondsRealtime(2);

        yield return FadeText(generalHelpText, 1, 0.02f);

        yield return new WaitForSeconds(2f);

        yield return FadeText(generalHelpText, 0, 0.01f);

        generalHelpText.text = "THERE ARE OBSTACLES ON THE WAY...";

        yield return FadeText(generalHelpText, 1, 0.01f);

        yield return new WaitForSeconds(2f);

        yield return FadeText(generalHelpText, 0, 0.01f);

        helpPanelFlashLeft.SetActive(true);

        obstacleManagerObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        yield return ObstacleSpawn(stackOne, 0.5f, 0, 1);
        StartCoroutine(DestoryObstacle(stackOne));

        yield return ObstacleSpawn(stackTwo, -0.5f, 1, 2);
        StartCoroutine(DestoryObstacle(stackTwo));

        obstacleHelpTextLeft.text = "SOME OBSTACLES REQUIRE 2 TAPS";
        
        yield return ObstacleSpawn(stackThree, 1f, 0, 2);
        StartCoroutine(DestoryObstacle(stackThree));

        obstacleHelpTextRight.text = "THE HARDEST ONES WILL NEED THREE TAPS";
        
        yield return ObstacleSpawn(stackFour, -1.5f, 1, 4);
        StartCoroutine(DestoryObstacle(stackFour));

        GameObject Comet2 = Instantiate(Comet, new Vector3(1f, -1f, 0f), Quaternion.identity);
        Comet2.GetComponent<CometMove>().gameManagerObject = gameManagerObject;

        yield return FadeText(starbieHelpText, 1, 0.02f);
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        yield return FadeText(starbieHelpText, 0, 0.01f);

        yield return new WaitForSeconds(1.5f);

        yield return FadeText(endText, 1, 0.01f);

        Asteroid.GetComponent<AsteroidRotation>().enabled = false;
        StartCoroutine(AsteroidRotation(5f));
        asteroidHolder.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -70f * Time.deltaTime);

        yield return new WaitForSecondsRealtime(4f);
        
        while (fadeButton.GetComponent<CanvasGroup>().alpha < 1)
        {
            fadeButton.GetComponent<CanvasGroup>().alpha += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        SceneManager.LoadScene(sceneName: "Game");
    }

}
