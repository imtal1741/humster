using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakGlassTrigger : MonoBehaviour
{

    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Foot"))
        {
            StartCoroutine(BreakGlass(other));
        }
    }

    IEnumerator BreakGlass(Collider other)
    {
        AgentGlassLogic agentGlassLogic = other.GetComponent<AgentGlassLogic>();
        if (agentGlassLogic)
        {
            agentGlassLogic.Dead();
        }

        if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(2f);
            meshRenderer.enabled = true;
        }
    }
}
