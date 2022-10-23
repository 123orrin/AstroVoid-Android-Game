using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem
{
    public static void SaveBuyData(BuyBools buyBools)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/buydata.json";
        FileStream stream = new FileStream(path, FileMode.Create);

        BuyData data = new BuyData(buyBools);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static BuyData LoadBuyData()
    {
        string path = Application.persistentDataPath + "/buydata.json";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            BuyData data = formatter.Deserialize(stream) as BuyData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Savefile not found in " + path);
            return null;
        }
    }
}
