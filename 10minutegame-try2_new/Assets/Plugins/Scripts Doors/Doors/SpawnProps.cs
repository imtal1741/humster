using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnProps : MonoBehaviour
{

    public Transform propsPoint;
    public PropSet propSet;

    public List<Interaction> interactions = new List<Interaction>();

    GameObject savedKeyObj;
    Interaction savedKeyInteraction;

    public void DoSpawn(bool isKey)
    {
        if (isKey)
        {
            if (interactions.Count > 0 && Random.Range(0, 100) < 70)
            {
                Interaction tempIntercation = interactions[Random.Range(0, interactions.Count)];
                tempIntercation.isKey = true;

                savedKeyInteraction = tempIntercation;
            }
            else
            {
                GameObject m = Instantiate(propSet.key,
                propsPoint.position + new Vector3(Random.Range(-0.075f, 0.075f), 0, Random.Range(-0.075f, 0.075f)),
                Quaternion.Euler(0, Random.Range(0, 360), 0));

                m.transform.SetParent(propsPoint);

                savedKeyObj = m;

                return;
            }
        }

        for (int i = 0; i < propSet.props.Length; i++)
        {
            int rand = Random.Range(0, 100);
            if (rand <= propSet.props[i].chance)
            {
                GameObject m = Instantiate(propSet.props[i].propPrefab[Random.Range(0, propSet.props[i].propPrefab.Length)],
                    propsPoint.position + new Vector3(Random.Range(-0.075f, 0.075f), 0, Random.Range(-0.075f, 0.075f)),
                    Quaternion.Euler(0, Random.Range(0, 360), 0));

                m.transform.SetParent(propsPoint);

                break;
            }
        }
    }

    public void DeleteKey()
    {
        if (savedKeyObj)
        {
            Destroy(savedKeyObj);
        }
        else if (savedKeyInteraction)
        {
            savedKeyInteraction.isKey = false;
        }

    }

}
