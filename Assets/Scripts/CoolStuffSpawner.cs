using System.Collections;
using UnityEngine;
using TMPro;

public class CoolStuffSpawner : MonoBehaviour
{
    public GameObject gameManagerObject;
    private GameManager gameManager;

    public GameObject Comet;
    public GameObject obstacleManagerObject;

    public int spawnPlanet;
    public GameObject Mercury;
    public float mercuryScore;
    public GameObject Venus;
    public float venusScore;
    public GameObject Earth;
    public float earthScore;
    public GameObject Mars;
    public float marsScore;
    public GameObject Jupiter;
    public float jupiterScore;
    public GameObject Saturn;
    public float saturnScore;
    public GameObject Uranus;
    public float uranusScore;
    public GameObject Neptune;
    public float neptuneScore;

    public TMP_Text incomingText;

    public GameObject starSpawn;

    // Start is called before the first frame update
    void Start()
    {
        //spawnPlanet = 0;
        gameManager = gameManagerObject.GetComponent<GameManager>();
        StartCoroutine(SpawnComet());
    }

    void Update()
    {
        if (gameManager.score > mercuryScore && spawnPlanet == 0)
        {
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Mercury, new Vector3(gameManager.minX + 1f, gameManager.minY - 2.3f, 11f), Quaternion.identity);
            Mercury.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_wise_wanderer, 100.00f, (success) => { 
                if (!success)
                {
                    Debug.Log("Could not give achievement");
                }
            });
        }

        if (gameManager.score > venusScore && spawnPlanet == 1)
        {
            incomingText.text = "!VENUS!";
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Venus, new Vector3(gameManager.maxX - 0.5f, gameManager.minY - 2.3f, 11f), Quaternion.identity);
            Venus.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_venerable_voyager, 100.00f, (success) => { });
        }

        if (gameManager.score > earthScore && spawnPlanet == 2)
        {
            incomingText.text = "!EARTH!";
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Earth, new Vector3(gameManager.minX + 1.5f, gameManager.minY - 2.3f, 11f), Quaternion.identity);
            Earth.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_astounding_adventurer, 100.00f, (success) => { });
        }

        if (gameManager.score > marsScore && spawnPlanet == 3)
        {
            incomingText.text = "!MARS!";
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Mars, new Vector3(gameManager.minX + 0.65f, gameManager.minY - 2.3f, 11f), Quaternion.identity);
            Mars.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_epic_excursionist, 100.00f, (success) => { });
        }

        if (gameManager.score > jupiterScore && spawnPlanet == 4)
        {
            incomingText.text = "!JUPITER!";
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Jupiter, new Vector3(gameManager.maxX - 1f, gameManager.minY - 3.4f, 11f), Quaternion.identity);
            Jupiter.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_valiant_vagabond, 100.00f, (success) => { });
        }

        if (gameManager.score > saturnScore && spawnPlanet == 5)
        {
            incomingText.text = "!SATURN!";
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Saturn, new Vector3(gameManager.minX + 0.5f, gameManager.minY - 2.3f, 11f), Quaternion.identity);
            Saturn.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_serious_sailor, 100.00f, (success) => { });
        }

        if (gameManager.score > uranusScore && spawnPlanet == 6)
        {
            incomingText.text = "!URANUS!";
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Uranus, new Vector3(gameManager.minX + 0.8f, gameManager.minY - 2.3f, 11f), Quaternion.identity);
            Uranus.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_talented_traveller, 100.00f, (success) => { });
        }

        if (gameManager.score > neptuneScore && spawnPlanet == 7)
        {
            incomingText.text = "!NEPTUNE!";
            StartCoroutine(FadeText(incomingText, 0.005f));
            Instantiate(Neptune, new Vector3(gameManager.maxX - 0.5f, gameManager.minY - 2.3f, 11f), Quaternion.identity);
            Neptune.GetComponent<PlanetMove>().gameManagerObject = gameManagerObject;
            spawnPlanet += 1;
            Social.ReportProgress(GPGSIds.achievement_experienced_explorer, 100.00f, (success) => { });
        }
    }

    IEnumerator SpawnComet()
    {
        while (starSpawn.GetComponent<ParticleSystem>().emission.enabled == true)
        {
            int spawn = Random.Range(1, 2);
            if (spawn == 1)
            {
                Instantiate(Comet, new Vector3(Random.Range(gameManager.minX - 2.5f, gameManager.minX - 0.5f), Random.Range(gameManager.minY - 0.5f, 0), 12f), Quaternion.identity);
                Comet.GetComponent<CometMove>().gameManagerObject = gameManagerObject;
            }
            if (spawn == 2)
            {
                Instantiate(Comet, new Vector3(Random.Range(gameManager.maxX + 0.5f, gameManager.maxX + 2.5f), Random.Range(gameManager.minY - 0.5f, 0), 12f), Quaternion.identity);
                Comet.GetComponent<CometMove>().gameManagerObject = gameManagerObject;
            }

            yield return new WaitForSecondsRealtime(8);
        }

        yield return null;
    }

    IEnumerator FadeText(TMP_Text text, float fadeTime)
    {
        while (text.GetComponent<CanvasGroup>().alpha < 1f)
        {
            text.GetComponent<CanvasGroup>().alpha += 0.02f;
            yield return new WaitForSecondsRealtime(fadeTime);
        }
        yield return null;
       
        while (text.GetComponent<CanvasGroup>().alpha > 0f)
        {
            text.GetComponent<CanvasGroup>().alpha -= 0.02f;
            yield return new WaitForSecondsRealtime(fadeTime);
        }
        yield return null;

        while (text.GetComponent<CanvasGroup>().alpha < 1f)
        {
            text.GetComponent<CanvasGroup>().alpha += 0.02f;
            yield return new WaitForSecondsRealtime(fadeTime);
        }
        yield return null;

        while (text.GetComponent<CanvasGroup>().alpha > 0f)
        {
            text.GetComponent<CanvasGroup>().alpha -= 0.02f;
            yield return new WaitForSecondsRealtime(fadeTime);
        }
        yield return null;
    }
}
