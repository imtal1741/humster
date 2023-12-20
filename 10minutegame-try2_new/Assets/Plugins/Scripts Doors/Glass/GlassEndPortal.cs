using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassEndPortal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        AgentGlassLogic agentGlassLogic = other.GetComponent<AgentGlassLogic>();
        if (agentGlassLogic)
        {
            agentGlassLogic.Dead(0f);
        }
    }
}
