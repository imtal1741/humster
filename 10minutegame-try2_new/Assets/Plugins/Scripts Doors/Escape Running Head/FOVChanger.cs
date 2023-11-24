using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class FOVChanger : MonoBehaviour
{

    public AdvancedWalkerController advancedWalkerController;
    public Camera cam;

    float maxMovementSpeed;
    float inputValue;
    float startFOV;
    public float addFOV;


    void Start()
    {
        maxMovementSpeed = advancedWalkerController.movementSpeed;
        startFOV = cam.fieldOfView;
    }

    void FixedUpdate()
    {
        bool isForward = advancedWalkerController.inputY > 0 ? true : false;
        inputValue = Mathf.Lerp(inputValue, isForward ? advancedWalkerController.inputY : 0, Time.deltaTime * (isForward ? 3 : 8));
        cam.fieldOfView = startFOV + (inputValue * addFOV);
    }
}
