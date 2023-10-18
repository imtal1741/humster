using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class AddForceRotate : MonoBehaviour
{
    public enum RotationAxis
    {
        x,
        y,
        z
    }
    public enum RotationDirection
    {
        forward,
        right
    }
    public RotationAxis currentRotationAxis = RotationAxis.x;
    public RotationDirection currentRotationDirection = RotationDirection.forward;

    AdvancedWalkerController advancedWalkerController;
    float lastRot;
    float dir;
    Vector3 resultMomentum;
    public float kickPower;

    void FixedUpdate()
    {
        float nowRot = 0;

        if (currentRotationAxis == RotationAxis.x)
            nowRot = transform.rotation.x;
        else if (currentRotationAxis == RotationAxis.y)
            nowRot = transform.rotation.y;
        if (currentRotationAxis == RotationAxis.z)
            nowRot = transform.rotation.z;


        dir = nowRot - lastRot;


        lastRot = nowRot;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!advancedWalkerController)
                advancedWalkerController = other.gameObject.GetComponent<AdvancedWalkerController>();

            if (!advancedWalkerController)
                advancedWalkerController = other.transform.parent.GetComponent<AdvancedWalkerController>();

            if (!advancedWalkerController)
                return;


            Vector3 rotDir = Vector3.zero;
            if (currentRotationDirection == RotationDirection.forward)
                rotDir = dir * transform.forward;
            else if (currentRotationDirection == RotationDirection.right)
                rotDir = dir * transform.right;

            rotDir = -rotDir * kickPower;
            resultMomentum = new Vector3(rotDir.x, 0, rotDir.z);
            Debug.Log(resultMomentum);

            advancedWalkerController.OnJumpInertia();
            advancedWalkerController.AddMomentum(resultMomentum);
        }
    }

}
