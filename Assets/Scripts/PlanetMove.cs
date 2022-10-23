using UnityEngine;

public class PlanetMove : MonoBehaviour
{
    public GameObject gameManagerObject;

    [SerializeField]
    private float speed;
    
    [SerializeField]
    private float rotation;

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, speed * Time.deltaTime);
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = rotation * Time.deltaTime;

        if (transform.position.y > gameManagerObject.GetComponent<GameManager>().maxY + 3.4f)
        {
            Destroy(gameObject);
        }
    }
}
