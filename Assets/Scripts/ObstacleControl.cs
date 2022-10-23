using System.Collections;
using UnityEngine;

public class ObstacleControl : MonoBehaviour
{
    public GameObject gameManagerObject;
    public GameObject obstacleManager;
    private GameManager gameManager;

    private Vector2 targetPos;
    private float moveAmount = 0.5f;
    private float corrector = 0.5f;

    public GameObject gameStack;

    public AudioSource invalidObstacleSFX;
    public AudioSource obstacleMoveSFX;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Camera.main.ScreenToWorldPoint(Input.touches[0].position).x > 0 && Input.touches[0].phase == TouchPhase.Began && gameObject.transform.position.y > gameManager.minY - 0.5f)
        {
            if (gameManager.maxX > gameObject.transform.position.x + moveAmount + corrector)
            {
                targetPos = new Vector3(gameObject.transform.position.x + moveAmount, transform.position.y, transform.position.z);
                transform.position = targetPos;
                obstacleMoveSFX.Play();
            }
            else
            {
                invalidObstacleSFX.Play();

                gameManager.StartCoroutine(gameManager.Shake(0.2f, 0.05f));
            }
        }

        if (Input.touchCount > 0 && Camera.main.ScreenToWorldPoint(Input.touches[0].position).x < 0 && Input.touches[0].phase == TouchPhase.Began && gameObject.transform.position.y > gameManager.minY - 0.5f)
        {
            if (gameManager.minX < gameObject.transform.position.x - moveAmount - corrector)
            {
                targetPos = new Vector3(transform.position.x - moveAmount, transform.position.y, transform.position.z);
                transform.position = targetPos;
                obstacleMoveSFX.Play();               
            }
            else
            {
                invalidObstacleSFX.Play();                
                gameManager.StartCoroutine(gameManager.Shake(0.2f, 0.05f));
            }
        }
        
        if (transform.position.x == 0f)
        {
            obstacleManager.GetComponent<ObstacleManager>().counter += 1;
            this.gameObject.GetComponent<ObstacleControl>().enabled = false;
        }

        if (obstacleManager.GetComponent<ObstacleManager>().counter == 9)
        {
            obstacleManager.GetComponent<ObstacleManager>().counter = 1;
        }
/*

###THIS CODE IS FOR DEBUGGING PURPOSES. WHEN DEVELOPING ON THE COMPUTER, UNCOMMENT THIS CODE TO CONTORL THE GAME USING ARROW KEYS

#if UNITY_EDITOR
        if (((Input.GetKeyDown(KeyCode.RightArrow) || (Input.GetMouseButtonDown(0) && Input.mousePosition.x > 190f))) && gameObject.transform.position.y > gameManager.minY - 0.5f)
        {
            if (gameManager.maxX > gameObject.transform.position.x + moveAmount + corrector)
            {
                obstacleMoveSFX.Play();
                targetPos = new Vector3(transform.position.x + moveAmount, transform.position.y, transform.position.z);
                transform.position = targetPos;
            }
            else
            {
                invalidObstacleSFX.Play();

                gameManager.StartCoroutine(gameManager.Shake(0.2f, 0.05f));
            }
        }

        if (((Input.GetKeyDown(KeyCode.LeftArrow) || (Input.GetMouseButtonDown(0) && Input.mousePosition.x < 190f))) && gameObject.transform.position.y > gameManager.minY - 0.5f)
        {
            if (gameManager.minX < gameObject.transform.position.x - moveAmount - corrector)
            {
                obstacleMoveSFX.Play();
                targetPos = new Vector3(transform.position.x - moveAmount, transform.position.y, transform.position.z);
                transform.position = targetPos;
            }
            else
            {
                invalidObstacleSFX.Play();

                gameManager.StartCoroutine(gameManager.Shake(0.2f, 0.05f));
            }
        }
#endif */
    }
}
