using System.Collections;
using UnityEngine;

public class OpenLink : MonoBehaviour
{

    public GameObject[] mobileNotFriendlyGames;

    [HideInInspector] public string domainName;
    [HideInInspector] public string langName = "ru";

    public void OpenURL(string gameId)
    {
        Application.OpenURL("https://yandex." + domainName + "/games/" + langName + "/app/" + gameId + "?lang=" + langName);
    }


    public void DisableIcons()
    {
        for (int i = 0; i < mobileNotFriendlyGames.Length; i++)
        {
            mobileNotFriendlyGames[i].SetActive(false);
        }
    }

}
