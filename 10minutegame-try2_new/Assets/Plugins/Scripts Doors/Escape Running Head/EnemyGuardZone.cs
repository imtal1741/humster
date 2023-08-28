using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGuardZone : MonoBehaviour
{

    [Header("Main Settings")]
    public Animator anim;
    public NavMeshAgent agent;
    public Transform player;
    public TriggerZone triggerZone;

    [Header("Alert")]
    public GameObject AlertObject;

    float randomTime;
    Vector3 startPos;
    bool isAggro;

    void Start()
    {
        startPos = transform.position;
        AlertObject.SetActive(false);

        randomTime = Random.Range(0.3f, 0.5f);
        StartCoroutine(FindPlayerIE());
    }

    IEnumerator FindPlayerIE()
    {
        
        yield return new WaitForSeconds(randomTime);


        if (triggerZone.isActive)
        {
            isAggro = true;
            AlertObject.SetActive(true);

            agent.destination = player.position;
            anim.SetBool("Move", true);
        }
        else if (isAggro)
        {
            isAggro = false;
            AlertObject.SetActive(false);

            agent.destination = startPos;
        }

        if (!agent.hasPath)
            anim.SetBool("Move", false);


        StartCoroutine(FindPlayerIE());

    }

}
