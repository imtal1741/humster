using System.Collections;
using UnityEngine;

public class OpenLink : MonoBehaviour
{

    public GameObject[] mobileNotFriendlyGames;

    [HideInInspector]
    public string domainName;

    public void OpenURL(string link)
    {
        Application.OpenURL("https://yandex." + domainName + link);
    }


    public void DisableIcons()
    {
        for (int i = 0; i < mobileNotFriendlyGames.Length; i++)
        {
            mobileNotFriendlyGames[i].SetActive(false);
        }
    }

}
