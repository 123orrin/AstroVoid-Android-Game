using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public GameObject gameManagerObject;

    public TMP_Text gameOverScoreText;

    // Start is called before the first frame update
    void Start()
    {
        gameOverScoreText.text = gameManagerObject.GetComponent<GameManager>().score.ToString("N0") + " KM";
    }
}
