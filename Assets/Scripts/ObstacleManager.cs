using System.Collections;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public int counter;

    public float distanceBetweenObstacles;

    public float speed;
    public float levelOneSpeed;
    public float levelTwoSpeed;
    public float levelThreeSpeed;
    public float levelFourSpeed;
    public float levelFiveSpeed;
    public float levelSixSpeed;
    public float levelSevenSpeed;
    public float levelEightSpeed;

    public int ScoreOne;
    public int ScoreTwo;
    public int ScoreThree;
    public int ScoreFour;
    public int ScoreFive;
    public int ScoreSix;
    public int ScoreSeven;
    public int ScoreEight;

    public Vector2 testSpeed;

    public GameObject gameManager;

    public Sprite[] spriteArray;

    private IEnumerator SpeedUpOne()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 1f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 1f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelOneSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return null;
    }
    private IEnumerator SpeedUpTwo()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0.8f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 0.8f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelTwoSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return null;
    }
    private IEnumerator SpeedUpThree()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0.7f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 0.7f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelThreeSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return null;
    }

    private IEnumerator SpeedUpFour()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0.6f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 0.6f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelFourSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return null;
    }

    private IEnumerator SpeedUpFive()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0.5f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 0.5f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelFiveSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(1f);
        }
        yield return null;
    }

    private IEnumerator SpeedUpSix()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0.6f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 0.6f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelSixSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(1f);
        }
        yield return null;
    }

    private IEnumerator SpeedUpSeven()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0.7f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 0.7f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelSevenSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(1f);
        }
        yield return null;
    }

    private IEnumerator SpeedUpEight()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0.8f)
        {
            gameManager.GetComponent<GameManager>().scoreFactor = 0.8f;
        }

        for (float speed = gameObject.GetComponent<ObstacleManager>().speed; speed < levelEightSpeed; speed += 0.03f)
        {
            gameObject.GetComponent<ObstacleManager>().speed = speed;
            yield return new WaitForSecondsRealtime(1f);
        }
        yield return null;
    }

    private void Update()
    {
        if (gameManager.GetComponent<GameManager>().scoreFactor != 0)
        {
            if (gameManager.GetComponent<GameManager>().score > ScoreOne)
            {
                StartCoroutine(SpeedUpOne());
            }
            if (gameManager.GetComponent<GameManager>().score > ScoreTwo)
            {
                StartCoroutine(SpeedUpTwo());
            }
            if (gameManager.GetComponent<GameManager>().score > ScoreThree)
            {
                StartCoroutine(SpeedUpThree());
            }
            if (gameManager.GetComponent<GameManager>().score > ScoreFour)
            {
                StartCoroutine(SpeedUpFour());
            }
            if (gameManager.GetComponent<GameManager>().score > ScoreFive)
            {
                StartCoroutine(SpeedUpFive());
            }
            if (gameManager.GetComponent<GameManager>().score > ScoreSix)
            {
                StartCoroutine(SpeedUpSix());
            }
            if (gameManager.GetComponent<GameManager>().score > ScoreSeven)
            {
                StartCoroutine(SpeedUpSeven());
            }
            if (gameManager.GetComponent<GameManager>().score > ScoreEight)
            {
                StartCoroutine(SpeedUpEight());
            }
        }
    }
}
