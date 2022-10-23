using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    public float speed;

    public GameObject asteroidHolder;

    private string spriteName;

    // Start is called before the first frame update
    void Start()
    {
        spriteName = gameObject.GetComponent<SpriteRenderer>().sprite.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteName == "myAsteroid")
        {
            transform.Rotate(new Vector3(0f, 0f, speed) * Time.deltaTime);
        }

        if (asteroidHolder.transform.localPosition.y > 0)
        {
            asteroidHolder.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -70f * Time.deltaTime);
        }
        else
        {
            asteroidHolder.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
    }    
}
