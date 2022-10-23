using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StarbieDisplay : MonoBehaviour
{
    public GameObject gameManagerObject;
    private GameManager gameManager;

    public TMP_Text starbiesText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        starbiesText.text = "<sprite=0> " + gameManager.starbies.ToString("N0");
    }
}
