using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubObjTrigger : MonoBehaviour
{

    public float radiusFactor;
    public float moveFactor;

    Rigidbody rb;


    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!rb)
                rb = other.GetComponent<Rigidbody>();

            if (radiusFactor != 0)
            {
                Vector3 playerPos = new Vector3(rb.transform.position.x, transform.position.y, rb.transform.position.z);
                float dist = Vector3.Distance(playerPos, transform.position);
                Vector3 playerPosVector = Vector3.ClampMagnitude((playerPos - transform.position), dist);
                Vector3 playerPosAdded = Vector3.ClampMagnitude((Quaternion.AngleAxis(1, Vector3.up) * playerPosVector), dist);

                Vector3 direction = (playerPosAdded - playerPosVector);
                rb.AddForce(radiusFactor * 2413 * direction);
            }
            else
            {
                rb.AddForce(moveFactor * 2413 * transform.forward);
            }
        }
    }


}
