using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosFollow : MonoBehaviour
{
    public Transform target;
    [Range(1f, 75f)]
    public float smoothTime = 30f;



    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * smoothTime);
    }
}
