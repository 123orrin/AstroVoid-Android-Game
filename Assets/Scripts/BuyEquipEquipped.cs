using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyEquipEquipped : MonoBehaviour
{
    public GameObject saveManagerObject;

    public GameObject gameManagerObject;
    private GameManager gameManager;

    public GameObject buttonManagerObject;

    [SerializeField]
    private int price;
    public TMP_Text priceText;

    public TMP_Text starbiesText;

    public TMP_Text buyEquipEquippedText;

    public bool bought;

    public int spriteNumber;

    void Start()
    {        
        gameManager = gameManagerObject.GetComponent<GameManager>();
        priceText.text = price.ToString();
    }

    public void BuyEquipEquippedButton()
    {
        if (bought == false)
        {
            if (gameManager.starbies >= price)
            {
                bought = true;
                gameManager.starbies -= price;
                PlayerPrefs.SetInt("starbies", gameManager.starbies);
                buttonManagerObject.GetComponent<AllButtons>().buySFX.Play();

                var color = gameObject.GetComponent<Button>().colors;
                color.normalColor = Color.red;
                buyEquipEquippedText.text = "Equipped";
                PlayerPrefs.SetInt("SpriteNumberInUse", spriteNumber);
                SaveSystem.SaveBuyData(saveManagerObject.GetComponent<BuyBools>());
            }
            else
            {
                StartCoroutine(CannotBuy());
            }
        }
        else
        {
            var color = gameObject.GetComponent<Button>().colors;
            color.normalColor = Color.grey;
            buyEquipEquippedText.text = "Equipped";
            buttonManagerObject.GetComponent<AllButtons>().equipSFX.Play();

            PlayerPrefs.SetInt("SpriteNumberInUse", spriteNumber);
        }
    }

    private void Update()
    {
        if (bought == true && PlayerPrefs.GetInt("SpriteNumberInUse", 0) != spriteNumber)
        {
            var color = gameObject.GetComponent<Button>().colors;
            color.normalColor = Color.magenta;
            buyEquipEquippedText.text = "Equip";
        }
        if (bought == true && PlayerPrefs.GetInt("SpriteNumberInUse", 0) == spriteNumber)
        {
            var color = gameObject.GetComponent<Button>().colors;
            color.normalColor = Color.grey;
            buyEquipEquippedText.text = "Equipped";
        }
    }

    IEnumerator CannotBuy()
    {
        Color color = starbiesText.color;
        int x = 0;
        buttonManagerObject.GetComponent<AllButtons>().invalidSFX.Play();

        while (x != 3)
        {
            starbiesText.CrossFadeColor(Color.red, 0.1f, true, false);
            yield return new WaitForSeconds(0.1f);
            starbiesText.CrossFadeColor(color, 0.1f, true, false);
            yield return new WaitForSeconds(0.1f);
            x += 1;
        }
        yield return null;
    }
}
