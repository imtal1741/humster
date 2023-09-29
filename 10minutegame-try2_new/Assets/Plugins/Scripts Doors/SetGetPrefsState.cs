using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGetPrefsState : MonoBehaviour
{

    public string stateName;
    public bool needTeleportThisObject;

    public void TeleportState(Transform tr)
    {
        if (needTeleportThisObject == false)
            return;

        if (GetState() == 1)
        {
            transform.position = tr.position;
        }
    }


    public void SaveState(int x)
    {
        PlayerPrefs.SetInt(stateName, x);
        PlayerPrefs.Save();
    }

    int GetState()
    {
        return PlayerPrefs.GetInt(stateName);
    }

}
