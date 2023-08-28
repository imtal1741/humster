using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{

    [Header("If Shooter")]
    public bool doDead;

    [Header("If doDead == false. Restart Scene or not")]
    public bool restartScene;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Health health = other.gameObject.GetComponent<Health>();

            if (doDead)
            {
                health.TakeDamage(999999);
            }
            else
            {
                health.Respawn(restartScene, gameObject.name);
            }
        }
    }


}
