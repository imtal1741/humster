using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDelete : MonoBehaviour
{
    public int Seconds = 6;

    void Start()
    {
        Destroy(gameObject, Seconds);
    }

}
