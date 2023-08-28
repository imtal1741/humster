using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MobSet", menuName = "MobSet")]

public class MobSet : ScriptableObject
{

    public MobClass[] mobs;

}

[System.Serializable]
public class MobClass
{

    public GameObject mobprefab;
    public bool isDistant;
    public int count;

}