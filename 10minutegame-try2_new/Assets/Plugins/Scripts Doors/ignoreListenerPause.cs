using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreListenerPause : MonoBehaviour
{


    void Start()
    {
        GetComponent<AudioSource>().ignoreListenerPause = true;
    }


}
