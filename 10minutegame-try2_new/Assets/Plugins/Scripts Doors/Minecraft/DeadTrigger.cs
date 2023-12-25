using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{

    public enum TagCheck
    {
        Player,
        Foot
    }
    public TagCheck currentTagCheck = TagCheck.Player;

    [Header("If need show respawn menu")]
    public bool doDead;

    [Header("If doDead == false. Restart Scene or not")]
    public bool restartScene;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(currentTagCheck.ToString()))
        {
            Health health = other.gameObject.GetComponent<Health>();

            if (!health)
                health = other.GetComponentInParent<Health>();

            if (!health)
                return;

            if (health.death)
                return;

            if (health.audioSource && health.deathSound)
                health.audioSource.PlayOneShot(health.deathSound, 0.5f);

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
