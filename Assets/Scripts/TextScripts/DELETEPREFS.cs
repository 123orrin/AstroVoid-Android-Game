using UnityEngine;

public class DELETEPREFS : MonoBehaviour
{
  public void Delete()
    {
        PlayerPrefs.DeleteAll();
    }
}
