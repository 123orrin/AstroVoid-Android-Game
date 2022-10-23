using UnityEngine;
using TMPro;

public class HighscoreDisplay : MonoBehaviour
{
    public GameObject gameManagerObject;

    public TMP_Text highscoreText;

    // Start is called before the first frame update
    void Start()
    {
        highscoreText.text = PlayerPrefs.GetFloat("highscore", 0).ToString("N0") + " KM";
    }
}
