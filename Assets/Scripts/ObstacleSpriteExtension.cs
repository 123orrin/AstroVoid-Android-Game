using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpriteExtension : MonoBehaviour
{
    public SpriteRenderer mainSprite;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<SpriteRenderer>().sprite != mainSprite.sprite)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = mainSprite.sprite;
        }
    }
}
