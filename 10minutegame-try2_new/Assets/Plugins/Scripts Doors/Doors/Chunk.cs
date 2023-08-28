using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("If Have CutScene")]
    public GameObject Timeline;

    public Transform Begin;
    public Transform Middle;
    public Transform End;

    public Animator animDoor;

    public int wallMaterialNumber;

    public List<MeshRenderer> Carpets = new List<MeshRenderer>();

    public List<GameObject> Plants = new List<GameObject>();

    public List<Transform> HighPoints = new List<Transform>();
    public List<Transform> LowPoints = new List<Transform>();


    private void Start()
    {
        //Spawn Plants
        if (Random.Range(0, 100) < 50)
        {
            for (int i = 0; i < Plants.Count; i++)
            {
                Plants[i].SetActive(false);
            }
        }
    }
}