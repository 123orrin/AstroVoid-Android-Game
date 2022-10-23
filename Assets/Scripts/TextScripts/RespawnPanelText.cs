using UnityEngine;
using TMPro;

public class RespawnPanelText : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text planetText;
    public GameObject coolStuffSpawnerObject;
    private CoolStuffSpawner coolStuffSpawner;

    public GameObject gameManagerObject;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        coolStuffSpawner = coolStuffSpawnerObject.GetComponent<CoolStuffSpawner>();
        gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameOver == true)
        {
            RespawnText();
        }
    }

    public void RespawnText()
    {
        switch (coolStuffSpawner.spawnPlanet)
        {
            case 0:
                scoreText.text = (coolStuffSpawner.mercuryScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "MERCURY";
                break;
            case 1:
                scoreText.text = (coolStuffSpawner.venusScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "VENUS";
                break;
            case 2:
                scoreText.text = (coolStuffSpawner.earthScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "EARTH";
                break;
            case 3:
                scoreText.text = (coolStuffSpawner.marsScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "MARS";
                break;
            case 4:
                scoreText.text = (coolStuffSpawner.jupiterScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "JUPITER";
                break;
            case 5:
                scoreText.text = (coolStuffSpawner.saturnScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "SATURN";
                break;
            case 6:
                scoreText.text = (coolStuffSpawner.uranusScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "URANUS";
                break;
            case 7:
                scoreText.text = (coolStuffSpawner.neptuneScore - gameManager.score).ToString("N0") + " KM";
                planetText.text = "NEPTUNE";
                break;
            case 8:
                scoreText.text = (100000000).ToString("N0") + " KM";
                planetText.text = "GALAXY ANDROMEDA";
                break;
        }
    }
}
