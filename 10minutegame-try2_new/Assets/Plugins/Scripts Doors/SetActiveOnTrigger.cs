using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetActiveOnTrigger : MonoBehaviour
{

    public GameObject InstantiatePrefab;
    public Transform InstantiateSpawnPoint;
    public List<GameObject> EnableObjects = new List<GameObject>();
    public List<GameObject> DisableObjects = new List<GameObject>();
    public UnityEvent m_Event;
    public AudioSource source;
    public AudioClip clip;

    bool used;

    void Start()
    {
        if (source)
            return;

        AudioSource camSource = Camera.main.GetComponent<AudioSource>();
        if (camSource)
            source = camSource;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (used)
                return;

            used = true;

            if (InstantiatePrefab)
                Instantiate(InstantiatePrefab, InstantiateSpawnPoint.position, InstantiateSpawnPoint.rotation);

            if (m_Event != null)
                m_Event.Invoke();

            if (source)
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
