using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class PreFallTrigger : MonoBehaviour
{


    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            AdvancedWalkerController advancedWalkerController = c.gameObject.GetComponent<AdvancedWalkerController>();

            advancedWalkerController.preFallTime = 0.2f;
        }
    }


    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            AdvancedWalkerController advancedWalkerController = c.gameObject.GetComponent<AdvancedWalkerController>();

            advancedWalkerController.preFallTime = 0;
        }
    }


}
