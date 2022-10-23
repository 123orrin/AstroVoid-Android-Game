using UnityEngine;
using GooglePlayGames;

public class CometMove : MonoBehaviour
{
    public GameObject gameManagerObject;
    private GameManager gameManager;

    public float touchdistance;

    public float xMov;
    public float yMov;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameManagerObject.GetComponent<GameManager>();

        if (transform.position.x < 0)
        {
            xMov = Random.Range(0.4f, 0.9f);
        }
        if (transform.position.x > 0)
        {
            xMov = Random.Range(-0.4f, -0.9f);
        }

        yMov = Random.Range(0.2f, 0.7f);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(xMov, yMov);
        var velocityOverLifetime = gameObject.GetComponent<ParticleSystem>().velocityOverLifetime;
        velocityOverLifetime.x = xMov * -3f;
        velocityOverLifetime.z = yMov * -3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerObject == null)
        {
            Destroy(gameObject);
        }

        if (gameManager.gameStart == true && gameManager.gameOver == false)
        {
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.touches[0].position), gameObject.transform.position) < touchdistance)
            {
                gameManager.starbies += 1;
                SendStarbieAchievementProgress();
                Destroy(gameObject);
            }
        }   
        
        if ((transform.position.x > gameManager.maxX + 5f) && xMov > 0f)
        {
            Destroy(gameObject);
        }
        if ((transform.position.x < gameManager.minX - 5f) && xMov < 0f)
        {
            Destroy(gameObject);
        }
    }

    private void SendStarbieAchievementProgress()
    {
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_novice_collector, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_amateur_collector, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_veteran_collector, 1, (success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_ultimate_collector, 1, (success) => { });
    }
}
