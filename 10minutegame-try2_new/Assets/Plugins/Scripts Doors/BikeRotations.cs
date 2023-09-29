using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class BikeRotations : MonoBehaviour
{
    public TurnTowardControllerVelocity turnTowardControllerVelocity;
    public AdvancedWalkerController advancedWalkerController;
    public Controller controller;

    public Transform[] Wheels;

    float startMovSpeed;
    float mainRotZ;

    void Start()
    {
        controller = advancedWalkerController.GetComponent<Controller>();

        startMovSpeed = advancedWalkerController.movementSpeed;
    }

    void FixedUpdate()
    {
        //Debug.Log(advancedWalkerController.savedMovementVelocity.magnitude);

        mainRotZ = Mathf.Lerp(mainRotZ, -8 * turnTowardControllerVelocity.factorRot, 5 * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, mainRotZ);
    }


    void RotateWheel()
    {
        //for (int i = 0; i < Wheels.Length; i++)
        //{
        //    Wheels
        //}


        //if (currentRotation > 360f)
        //    currentRotation -= 360f;
        //if (currentRotation < -360f)
        //    currentRotation += 360f;
    }

}
