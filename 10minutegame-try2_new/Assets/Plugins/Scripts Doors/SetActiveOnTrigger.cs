using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetActiveOnTrigger : MonoBehaviour
{

    public GameObject InstantiatePrefab;
    public List<GameObject> EnableObjects = new List<GameObject>();
    public List<GameObject> DisableObjects = new List<GameObject>();
    public UnityEvent m_Event;
    public AudioSource source;
    public AudioClip clip;

    bool used;

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (used)
                return;

            used = true;

            if (InstantiatePrefab)
                Instantiate(InstantiatePrefab, transform.position, transform.rotation);

            if (m_Event != null)
                m_Event.Invoke();

            source.PlayOneShot(clip);

            for (int i = 0; i < EnableObjects.Count; i++)
            {
                EnableObjects[i].SetActive(true);
            }

            for (int i = 0; i < DisableObjects.Count; i++)
            {
                DisableObjects[i].SetActive(false);
            }

            Destroy(gameObject);
        }
    }


}
