using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PropSet", menuName = "PropSet")]

public class PropSet : ScriptableObject
{

    public GameObject key;
    public PropClass[] props;

}

[System.Serializable]
public class PropClass
{
    public GameObject[] propPrefab;
    public int chance;
}