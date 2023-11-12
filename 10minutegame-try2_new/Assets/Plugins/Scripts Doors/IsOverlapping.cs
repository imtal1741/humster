using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsOverlapping : MonoBehaviour
{
    public int _overlaps;

    public bool isOverlapping
    {
        get
        {
            return _overlaps > 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        _overlaps++;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        _overlaps--;
    }
}
