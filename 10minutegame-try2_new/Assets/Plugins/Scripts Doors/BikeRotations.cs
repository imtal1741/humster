using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class BikeRotations : MonoBehaviour
{
    public TurnTowardControllerVelocity turnTowardControllerVelocity;
    public AdvancedWalkerController advancedWalkerController;

    public Transform[] Wheels;
    public Transform Handlebar;

    float startMovSpeed;
    float mainRotZ;
    float currentRotation;

    void Start()
    {
        startMovSpeed = advancedWalkerController.movementSpeed;
    }

    void FixedUpdate()
    {
        RotateWheel();

        mainRotZ = Mathf.Lerp(mainRotZ, -10 * turnTowardControllerVelocity.factorRot, 5 * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, mainRotZ * 2);
        Handlebar.localRotation = Quaternion.Euler(0, -mainRotZ * 4, 0);
    }


    void RotateWheel()
    {
        float speed = advancedWalkerController.GetVelocity().magnitude;
        currentRotation += speed;

        if (currentRotation > 360f)
            currentRotation -= 360f;
        if (currentRotation < -360f)
            currentRotation += 360f;

        for (int i = 0; i < Wheels.Length; i++)
        {
            Wheels[i].localRotation = Quaternion.Euler(currentRotation, 0, 0);
        }
    }

}
