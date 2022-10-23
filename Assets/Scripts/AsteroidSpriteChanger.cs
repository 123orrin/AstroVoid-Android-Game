using UnityEngine;

public class AsteroidSpriteChanger : MonoBehaviour
{
    public SpriteRenderer asteroidSprite;

    public Sprite[] spriteArray;
    public void ChangeSprite(int sprite)
    {
        asteroidSprite.sprite = spriteArray[sprite];
    }
}
