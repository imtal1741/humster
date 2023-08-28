using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MapSet", menuName = "MapSet")]

public class MapSet : ScriptableObject
{

    public MapClass[] maps;

}

[System.Serializable]
public class MapClass
{
    public GameObject[] mapPrefab;
    public float chanceMin;
    public float chanceMax;
}