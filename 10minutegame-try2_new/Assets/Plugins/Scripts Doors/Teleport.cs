using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Teleport : MonoBehaviour
{

    public Transform point;
    public UnityEvent m_Event;
    public AudioSource source;
    public AudioClip clip;

    Health health;

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {

            if (!health)
                health = c.gameObject.GetComponent<Health>();

            if (!health)
                return;

            if (m_Event != null)
                m_Event.Invoke();

            source.PlayOneShot(clip);
            health.StartCoroutine(health.TeleportIE(point));

        }
    }

}
