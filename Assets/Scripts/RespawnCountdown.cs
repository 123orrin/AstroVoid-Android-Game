using UnityEngine;
using TMPro;

public class RespawnCountdown : MonoBehaviour
{
    private float respawnTimer;
    public TMP_Text respawnTimerText;

    // Start is called before the first frame update
    void Start()
    {
        respawnTimer = 6;
    }

    // Update is called once per frame
    void Update()
    {
        respawnTimer -= Time.deltaTime;
        if (respawnTimer > 0)
        {
            respawnTimerText.text = (respawnTimer - Time.deltaTime).ToString("N0");
        }
        else
        {
            gameObject.GetComponent<RespawnCountdown>().enabled = false;
        }
    }
}
