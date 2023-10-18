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

    // Count how many colliders are overlapping this trigger.
    // If desired, you can filter here by tag, attached components, etc.
    // so that only certain collisions count. Physics layers help too.
    void OnTriggerEnter(Collider other)
    {
        _overlaps++;
    }

    void OnTriggerExit(Collider other)
    {
        _overlaps--;
    }
}
