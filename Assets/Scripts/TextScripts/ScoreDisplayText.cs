using UnityEngine;
using TMPro;

public class ScoreDisplayText : MonoBehaviour
{
    public GameObject gameManagerObject;
    private GameManager gameManager;

    public TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameManagerObject.GetComponent<GameManager>();

        scoreText.text = "<sprite=0> " + gameManager.highscore.ToString("N0") + " KM";
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetComponent<GameManager>().gameStart == true)
        {
            scoreText.text = gameManager.score.ToString("N0") + " KM";
        }
    }    
}
