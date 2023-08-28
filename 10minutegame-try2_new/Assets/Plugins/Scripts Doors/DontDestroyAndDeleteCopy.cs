using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyAndDeleteCopy : MonoBehaviour
{

    public static DontDestroyAndDeleteCopy instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
