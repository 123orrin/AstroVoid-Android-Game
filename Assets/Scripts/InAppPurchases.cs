using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchases : MonoBehaviour
{
    private string fiveHundredStarbies = "**CONFIDENTIAL**";
    private string twoThousandStarbies = "**CONFIDENTIAL**";
    private string fiveThousandStarbies = "**CONFIDENTIAL**";

    public GameObject gameManagerObject;

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == fiveHundredStarbies)
        {
            gameManagerObject.GetComponent<GameManager>().starbies += 500;
            PlayerPrefs.SetInt("starbies", gameManagerObject.GetComponent<GameManager>().starbies);
            Debug.Log("IAP 500");
        }

        if (product.definition.id == twoThousandStarbies)
        {
            gameManagerObject.GetComponent<GameManager>().starbies += 2000;
            PlayerPrefs.SetInt("starbies", gameManagerObject.GetComponent<GameManager>().starbies);
            Debug.Log("IAP 2000");
        }

        if (product.definition.id == fiveThousandStarbies)
        {
            gameManagerObject.GetComponent<GameManager>().starbies += 5000;
            PlayerPrefs.SetInt("starbies", gameManagerObject.GetComponent<GameManager>().starbies);
            Debug.Log("IAP 5000");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase of " + product.definition.id + " failed due to " + reason);
    }
}
