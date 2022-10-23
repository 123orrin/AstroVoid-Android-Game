using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Advertisements;

public class AllCollisions : MonoBehaviour
{
    public GameObject obstacleManagerObject;
    public GameObject gameManagerObject;
    public GameObject respawnCanvas;
    public GameObject gameOverCanvas;

    public GameObject stackOne;
    public GameObject stackTwo;
    public GameObject stackThree;
    public GameObject stackFour;
    public GameObject stackFive;
    public GameObject stackSix;
    public GameObject stackSeven;
    public GameObject stackEight;

    public GameObject Asteroid;
    public GameObject AsteroidReCreate;
    public GameObject AsteroidCreate;
    public GameObject AsteroidDeathExplosion;
    public float asteroidScaleX;

    public int respawnCount;
    public float respawnSpeed;

    public GameObject cam;

    public AudioSource explosionSFX;
    public AudioSource respawnSFX;

    public int starbiesEarnedThisRound;
    public TMP_Text starbiesEarnedText;

    public GameObject monetizationManager;

    private void Start()
    {
        asteroidScaleX = Asteroid.transform.localScale.x;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ObstacleChild")
        {
            PlayerPrefs.SetInt("starbies", gameManagerObject.GetComponent<GameManager>().starbies);
            starbiesEarnedThisRound = gameManagerObject.GetComponent<GameManager>().starbies - gameManagerObject.GetComponent<GameManager>().starbiesAtGameStart;
            starbiesEarnedText.text = starbiesEarnedThisRound.ToString("N0") + " STARBIES";

            gameManagerObject.GetComponent<GameManager>().gameOver = true;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(AsteroidDie());
            gameManagerObject.GetComponent<GameManager>().scoreFactor = 0f;
            obstacleManagerObject.GetComponent<ObstacleManager>().StopAllCoroutines();
            StartCoroutine(Shake(0.5f, 0.1f));
            StartCoroutine(SlowDown());
            gameManagerObject.GetComponent<GameManager>().StartCoroutine(FadeAudioSource.FadeAudio(gameManagerObject.GetComponent<GameManager>().gameAudio, 5f, 0.2f));
        }
    }

    IEnumerator AsteroidDie()
    {
        AsteroidDeathExplosion.SetActive(true);
        explosionSFX.Play();
        while (Asteroid.transform.localScale.x > 0)
        {
            Asteroid.transform.localScale -= new Vector3(0.05f, 0.05f, 0f);
            yield return new WaitForSecondsRealtime(0.05f);
        }
        Asteroid.SetActive(false);
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = cam.transform.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        cam.transform.localPosition = originalPos;
    }

    IEnumerator SlowDown()
    {
        respawnSpeed = obstacleManagerObject.GetComponent<ObstacleManager>().speed;
        while (obstacleManagerObject.GetComponent<ObstacleManager>().speed > 0)
        {
            obstacleManagerObject.GetComponent<ObstacleManager>().speed -= 0.03f;
            yield return new WaitForSeconds(0.01f);
        }

        if (respawnCount < 1)
        {
            StartCoroutine(RespawnCanvasFadeIn());
        }
        else
        {
            StartCoroutine(GameOverCanvasFadeIn());
        }
        yield return null;
    }
    public IEnumerator RespawnCanvasFadeIn()
    {
        respawnCanvas.SetActive(true);

        if (Advertisement.IsReady(monetizationManager.GetComponent<AdManager>().rewardedAd))
        {
            monetizationManager.GetComponent<AdManager>().respawnButton.interactable = true;
        }

        while (respawnCanvas.GetComponent<CanvasGroup>().alpha != 1f)
        {
            respawnCanvas.GetComponent<CanvasGroup>().alpha += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(5);
        StartCoroutine(GameOverCanvasFadeIn());
    }

    IEnumerator GameOverCanvasFadeIn()
    {
        gameOverCanvas.SetActive(true);

        if (Advertisement.IsReady(monetizationManager.GetComponent<AdManager>().dailyRewardAd))
        {
            monetizationManager.GetComponent<AdManager>().doubleStarbieButton.interactable = true;
        }

        while (gameOverCanvas.GetComponent<CanvasGroup>().alpha != 1)
        {
            gameOverCanvas.GetComponent<CanvasGroup>().alpha += 0.02f;
            respawnCanvas.GetComponent<CanvasGroup>().alpha -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        respawnCanvas.SetActive(false);
        yield return null;
    }

    public IEnumerator Respawn()
    {
        while (respawnCanvas.GetComponent<CanvasGroup>().alpha > 0)
        {
             respawnCanvas.GetComponent<CanvasGroup>().alpha -= 0.02f;
             yield return new WaitForSecondsRealtime(0.01f);
        }

         respawnCanvas.SetActive(false);

         respawnCount += 1;

         stackOne.GetComponent<Transform>().transform.position = new Vector3(0f, stackOne.GetComponent<Transform>().transform.position.y, stackOne.GetComponent<Transform>().transform.position.z);
         stackTwo.GetComponent<Transform>().transform.position = new Vector3(0f, stackTwo.GetComponent<Transform>().transform.position.y, stackTwo.GetComponent<Transform>().transform.position.z);
         stackThree.GetComponent<Transform>().transform.position = new Vector3(0f, stackThree.GetComponent<Transform>().transform.position.y, stackThree.GetComponent<Transform>().transform.position.z);
         stackFour.GetComponent<Transform>().transform.position = new Vector3(0f, stackFour.GetComponent<Transform>().transform.position.y, stackFour.GetComponent<Transform>().transform.position.z);
         stackFive.GetComponent<Transform>().transform.position = new Vector3(0f, stackFive.GetComponent<Transform>().transform.position.y, stackFive.GetComponent<Transform>().transform.position.z);
         stackSix.GetComponent<Transform>().transform.position = new Vector3(0f, stackSix.GetComponent<Transform>().transform.position.y, stackSix.GetComponent<Transform>().transform.position.z);
         stackSeven.GetComponent<Transform>().transform.position = new Vector3(0f, stackSeven.GetComponent<Transform>().transform.position.y, stackSeven.GetComponent<Transform>().transform.position.z);
         stackEight.GetComponent<Transform>().transform.position = new Vector3(0f, stackEight.GetComponent<Transform>().transform.position.y, stackEight.GetComponent<Transform>().transform.position.z);

        yield return StartCoroutine(ReCreateAsteroid());

        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        gameManagerObject.GetComponent<GameManager>().gameOver = false;

        gameManagerObject.GetComponent<GameManager>().StartCoroutine(FadeAudioSource.FadeAudio(gameManagerObject.GetComponent<GameManager>().gameAudio, 5f, 0.6f));

        gameManagerObject.GetComponent<GameManager>().scoreFactor = 1f;

        yield return null;
    }

    IEnumerator ReCreateAsteroid()
    {
        AsteroidReCreate.SetActive(true);
        respawnSFX.Play();
        yield return new WaitForSeconds(2);
        AsteroidCreate.SetActive(true);
        Asteroid.SetActive(true);
        StartCoroutine(SpeedUp());

        while (Asteroid.transform.localScale.x < asteroidScaleX)
        {
            Asteroid.transform.localScale += new Vector3(0.05f, 0.05f, 0f);
            yield return new WaitForSecondsRealtime(0.05f);
        }
        AsteroidReCreate.SetActive(false);
        yield return new WaitForSeconds(2);
        AsteroidCreate.SetActive(false);
    }

    IEnumerator SpeedUp()
    {
        while (obstacleManagerObject.GetComponent<ObstacleManager>().speed < respawnSpeed)
        {
            obstacleManagerObject.GetComponent<ObstacleManager>().speed += 0.01f;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
