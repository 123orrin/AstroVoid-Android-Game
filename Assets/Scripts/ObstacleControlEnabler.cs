using UnityEngine;

public class ObstacleControlEnabler : MonoBehaviour
{
    public int stackNumber;

    public GameObject obstacleManagerObject;
    private ObstacleManager obstacleManager;

    public GameObject referenceStack;
    public Rigidbody2D rb;
    
    public SpriteRenderer obstacleSpriteRight;
    public SpriteRenderer obstacleSpriteLeft;

    // Start is called before the first frame update
    void Start()
    {
        obstacleManager = obstacleManagerObject.GetComponent<ObstacleManager>();

        int initialPlacement = Random.Range(1, 7);

        switch (initialPlacement)
        {
            case 1:
                gameObject.transform.position = new Vector3(0.5f, transform.position.y, -14f);
                break;
            case 2:
                gameObject.transform.position = new Vector3(-0.5f, transform.position.y, -14f);
                break;
            case 3:
                gameObject.transform.position = new Vector3(1f, transform.position.y, -14f);
                break;
            case 4:
                gameObject.transform.position = new Vector3(-1f, transform.position.y, -14f);
                break;
            case 5:
                gameObject.transform.position = new Vector3(1.5f, transform.position.y, -14f);
                break;
            case 6:
                gameObject.transform.position = new Vector3(-1.5f, transform.position.y, -14f);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Staggered Control Code
        if (obstacleManager.counter == stackNumber)
        {
            gameObject.GetComponent<ObstacleControl>().enabled = true;
        }

        //Renew Obstacle Code
        if (gameObject.transform.position.y > 6f)
        {
            if (referenceStack.transform.position.y > -6f)
            {
                int placement = Random.Range(1, 7);

                switch (placement)
                {
                    case 1:
                        gameObject.transform.position = new Vector3(0.5f, referenceStack.transform.position.y - obstacleManager.distanceBetweenObstacles, -14f);
                        break;
                    case 2:
                        gameObject.transform.position = new Vector3(-0.5f, referenceStack.transform.position.y - obstacleManager.distanceBetweenObstacles, -14f);
                        break;
                    case 3:
                        gameObject.transform.position = new Vector3(1f, referenceStack.transform.position.y - obstacleManager.distanceBetweenObstacles, -14f);
                        break;
                    case 4:
                        gameObject.transform.position = new Vector3(-1f, referenceStack.transform.position.y - obstacleManager.distanceBetweenObstacles, -14f);
                        break;
                    case 5:
                        gameObject.transform.position = new Vector3(1.5f, referenceStack.transform.position.y - obstacleManager.distanceBetweenObstacles, -14f);
                        break;
                    case 6:
                        gameObject.transform.position = new Vector3(-1.5f, referenceStack.transform.position.y - obstacleManager.distanceBetweenObstacles, -14f);
                        break;
                }
            }
        }
        
        //Obstacle Velocity Code
        //rb.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, obstacleManager.speed * Time.deltaTime);

        //Obstacle Sprite Change

        float reference = Mathf.Abs(0 - transform.position.x);

        if (reference == 0f)
        {
            obstacleSpriteRight.sprite = obstacleManager.spriteArray[0];
            obstacleSpriteLeft.sprite = obstacleManager.spriteArray[0];
        }
        else if (reference <= 0.5f)
        {
            obstacleSpriteRight.sprite = obstacleManager.spriteArray[1];
            obstacleSpriteLeft.sprite = obstacleManager.spriteArray[1];
        }
        else if (reference <= 1f)
        {
            obstacleSpriteRight.sprite = obstacleManager.spriteArray[2];
            obstacleSpriteLeft.sprite = obstacleManager.spriteArray[2];
        }
        else if (reference <= 1.5f)
        {
            obstacleSpriteRight.sprite = obstacleManager.spriteArray[3];
            obstacleSpriteLeft.sprite = obstacleManager.spriteArray[3];
        }
        else
        {
            obstacleSpriteRight.sprite = obstacleManager.spriteArray[4];
            obstacleSpriteLeft.sprite = obstacleManager.spriteArray[4];
        }
    }

    private void FixedUpdate()
    {
        obstacleManager.testSpeed = new Vector2(0, obstacleManager.speed);
        rb.MovePosition(rb.position + obstacleManager.testSpeed * Time.deltaTime);
    }
}