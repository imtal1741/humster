using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDestroy : MonoBehaviour
{

    public GameObject particleDestroy;
    public GameObject[] wall;

    public void ActivateDestroy()
    {
        StartCoroutine(ActivateDestroyIE());
    }


    IEnumerator ActivateDestroyIE()
    {
        for (int i = 0; i < wall.Length; i++)
        {
            TextMeshPro tempText = wall[i].transform.GetChild(0).GetComponent<TextMeshPro>();
            for (int j = 49; j > 0; j--)
            {
                tempText.text = j.ToString();
                yield return new WaitForSeconds(.02f);
                if (j == 1)
                {
                    Instantiate(particleDestroy, wall[i].transform.position, wall[i].transform.rotation);
                    Destroy(wall[i]);
                }
            }
        }
        Destroy(gameObject);
    }



}
