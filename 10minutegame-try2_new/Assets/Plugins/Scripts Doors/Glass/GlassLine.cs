using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassLine : MonoBehaviour
{

    public Transform[] glasses;
    public int trueGlass = 0;
    public GlassLineManager glassLineManager;

    public float lastKnownTime = -9999;

    [HideInInspector] public Vector3 trueGlassCenter;
    [HideInInspector] public Vector3 trueGlassBounds;

    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        trueGlassCenter = mr.bounds.center;
        trueGlassBounds = mr.bounds.extents;
    }
}
