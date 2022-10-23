
[System.Serializable]
public class BuyData
{
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

    public BuyData(BuyBools buyBools)
    {
        defaultAsteroidBought = buyBools.defaultAsteroidBought;
        itemOneBought = buyBools.itemOneBought;
        itemTwoBought = buyBools.itemTwoBought;
        itemThreeBought = buyBools.itemThreeBought;
        itemFourBought = buyBools.itemFourBought;
        itemFiveBought = buyBools.itemFiveBought;
        itemSixBought = buyBools.itemSixBought;
        itemSevenBought = buyBools.itemSevenBought;
        itemEightBought = buyBools.itemEightBought;
        itemNineBought = buyBools.itemNineBought;
    }
}
