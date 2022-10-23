using UnityEngine;

public class BuyBools : MonoBehaviour
{
    public GameObject defaultAsteroidObject;
    public GameObject itemOneObject;
    public GameObject itemTwoObject;
    public GameObject itemThreeObject;
    public GameObject itemFourObject;
    public GameObject itemFiveObject;
    public GameObject itemSixObject;
    public GameObject itemSevenObject;
    public GameObject itemEightObject;
    public GameObject itemNineObject;


    public bool defaultAsteroidBought;
    public bool itemOneBought;
    public bool itemTwoBought;
    public bool itemThreeBought;
    public bool itemFourBought;
    public bool itemFiveBought;
    public bool itemSixBought;
    public bool itemSevenBought;
    public bool itemEightBought;
    public bool itemNineBought;


    private void Start()
    {
        BuyData data = SaveSystem.LoadBuyData();

        defaultAsteroidObject.GetComponent<BuyEquipEquipped>().bought = data.defaultAsteroidBought;
        itemOneObject.GetComponent<BuyEquipEquipped>().bought = data.itemOneBought;
        itemTwoObject.GetComponent<BuyEquipEquipped>().bought = data.itemTwoBought;
        itemThreeObject.GetComponent<BuyEquipEquipped>().bought = data.itemThreeBought;
        itemFourObject.GetComponent<BuyEquipEquipped>().bought = data.itemFourBought;
        itemSixObject.GetComponent<BuyEquipEquipped>().bought = data.itemFiveBought;
        itemSevenObject.GetComponent<BuyEquipEquipped>().bought = data.itemSixBought;
        itemEightObject.GetComponent<BuyEquipEquipped>().bought = data.itemSevenBought;
        itemNineObject.GetComponent<BuyEquipEquipped>().bought = data.itemEightBought;

    }

    // Update is called once per frame
    void Update()
    {
        defaultAsteroidBought = defaultAsteroidObject.GetComponent<BuyEquipEquipped>().bought;
        itemOneBought = itemOneObject.GetComponent<BuyEquipEquipped>().bought;
        itemTwoBought = itemTwoObject.GetComponent<BuyEquipEquipped>().bought;
        itemThreeBought = itemThreeObject.GetComponent<BuyEquipEquipped>().bought;
        itemFourBought = itemFourObject.GetComponent<BuyEquipEquipped>().bought;
        itemFiveBought = itemFiveObject.GetComponent<BuyEquipEquipped>().bought;
        itemSixBought = itemSixObject.GetComponent<BuyEquipEquipped>().bought;
        itemSevenBought = itemSevenObject.GetComponent<BuyEquipEquipped>().bought;
        itemEightBought = itemEightObject.GetComponent<BuyEquipEquipped>().bought;
        itemNineBought = itemNineObject.GetComponent<BuyEquipEquipped>().bought;

    }
}
