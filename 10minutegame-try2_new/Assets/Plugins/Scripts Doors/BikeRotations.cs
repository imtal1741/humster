using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class BikeRotations : MonoBehaviour
{
    [Header("Main Links")]
    public TurnTowardControllerVelocity turnTowardControllerVelocity;
    public AdvancedWalkerController advancedWalkerController;
    Controller controller;

    [Header("Audio")]
    public AudioSource source;
    public AudioSource wheelSource;
    public AudioClip wheelsSound;
    public AudioClip jumpSound;

    [Header("Other Transform Settings")]
    public Transform[] pedalRoot;
    public Transform[] pedal;
    public IsOverlapping FaceTrigger;

    [Header("Wheels Settings")]
    public LayerMask ignoreLayer;
    public Transform[] Wheels;
    public float WheelsSpeed;
    public Vector3 WheelRayDownOffset;
    public float WheelRayMaxDistance;
    public Transform Handlebar;
    public Transform Character;
    public float rotationClamp;

    float startMovSpeed;
    float mainPosY;
    float mainPosYNew;
    float mainRotX;
    float mainRotXNew;
    float mainRotY;
    float mainRotZ;
    float mainSlant;
    float HandlebarY;
    float currentRotation;
    float lastSavedRotation;
    bool isGrounded;

    Vector3[] wheelsHitPoint;

    void Start()
    {
        controller = advancedWalkerController.GetComponent<Controller>();
        controller.OnJump += OnJump;

        startMovSpeed = advancedWalkerController.movementSpeed;

        wheelsHitPoint = new Vector3[Wheels.Length];
    }

    void FixedUpdate()
    {
        isGrounded = advancedWalkerController.IsGrounded();

        RotateWheel();

        float needRotZ = Mathf.Clamp(Mathf.Lerp(mainRotZ, -18 * turnTowardControllerVelocity.factorRot, Time.fixedDeltaTime * 5), -rotationClamp, rotationClamp);
        mainRotZ = Mathf.Lerp(mainRotZ, needRotZ, Time.fixedDeltaTime * 25);

        for (int i = 0; i < Wheels.Length; i++)
        {
            Vector3 offset = i == 0 ? new Vector3(WheelRayDownOffset.x * -1, WheelRayDownOffset.y, WheelRayDownOffset.z * -1) : WheelRayDownOffset;

            if (isGrounded)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.parent.position + transform.parent.TransformDirection(offset), -Vector3.up, out hit, WheelRayMaxDistance, ~ignoreLayer))
                {
                    wheelsHitPoint[i] = hit.point;
                    Debug.DrawLine(transform.parent.position + transform.parent.TransformDirection(offset), wheelsHitPoint[i], Color.green);
                }
                else
                {
                    wheelsHitPoint[i] = transform.parent.position + transform.parent.TransformDirection(offset) - Vector3.up * (WheelRayDownOffset.y + 0.02f);
                }
            }
            else
            {
                wheelsHitPoint[i] = transform.parent.position + transform.parent.TransformDirection(offset) - Vector3.up * (WheelRayDownOffset.y + 0.02f);
            }
        }
        //Vector3 wheelNewPoint = transform.InverseTransformPointUnscaled(Wheels[1].position);
        //Vector3 wheelP = transform.parent.position + transform.parent.TransformDirection(new Vector3(0, 1.03f, wheelNewPoint.z));
        //Debug.DrawLine(wheelP, wheelP - Vector3.up, Color.red);

        Vector3 groundPoint = isGrounded ? advancedWalkerController.GetGroundPoint() : advancedWalkerController.transform.position;
        Vector3 differenceSmoothedPos = groundPoint - advancedWalkerController.transform.position;
        float medianY = (wheelsHitPoint[0].y + wheelsHitPoint[1].y) / 2;
        Vector3 dir = wheelsHitPoint[1] - wheelsHitPoint[0];
        float angle = Vector3.Angle(transform.parent.forward, dir);

        int PosNeg = dir.y > 0 ? -1 : 1;
        float offsetY = (medianY - groundPoint.y) + (differenceSmoothedPos.y * 1.5f);
        mainPosYNew = offsetY;
        mainRotXNew = angle * PosNeg;

        mainPosY = mainPosYNew;
        mainRotX = Mathf.Lerp(mainRotX, (isGrounded ? mainRotXNew : -15), Time.fixedDeltaTime * (isGrounded ? 15 : 5));
        float floatX = Mathf.Clamp(mainRotZ * 3, -20, 20);
        float inputX = advancedWalkerController.inputX;
        float inputY = advancedWalkerController.inputY;
        mainRotY = Mathf.Lerp(mainRotY, (isGrounded ? 0 : inputX == 0 ? inputY < 0 ? inputY * (70 * Mathf.Sign(floatX)) : floatX * -1.5f : inputX * 30 + floatX), Time.fixedDeltaTime * (isGrounded ? 10 : 5));
        mainSlant = Mathf.Lerp(mainSlant, (isGrounded ? mainRotZ * 1.5f : inputX == 0 ? floatX * -1.5f : inputX * 20 + floatX), Time.fixedDeltaTime * (isGrounded ? 20 : 10));
        HandlebarY = Mathf.Lerp(HandlebarY, (isGrounded ? -mainRotZ * 2.5f : inputX == 0 ? floatX * -1.5f : inputX * 30 + floatX), Time.fixedDeltaTime * (isGrounded ? 50 : 5));

        transform.localPosition = new Vector3(0, mainPosY, 0);
        transform.localRotation = Quaternion.Euler(mainRotX, mainRotY, mainSlant);
        Handlebar.localRotation = Quaternion.Euler(0, HandlebarY, 0);
        Character.localRotation = Quaternion.Euler(0, 0, -mainSlant * 0.9f);
    }


    void RotateWheel()
    {
        Vector3 tempVelocity = advancedWalkerController.GetVelocity() * (FaceTrigger.isOverlapping ? 0.5f : 1f);
        float speed = new Vector3(tempVelocity.x, 0, tempVelocity.z).magnitude;
        currentRotation += speed * WheelsSpeed;

        if (currentRotation > 360f)
            currentRotation -= 360f;
        if (currentRotation < -360f)
            currentRotation += 360f;


        for (int i = 0; i < pedalRoot.Length; i++)
        {
            if (isGrounded)
            {
                pedalRoot[i].localRotation = Quaternion.Euler(currentRotation, 0, 0);
                pedal[i].localRotation = Quaternion.Euler(-currentRotation, 0, 0);
            }
            else
            {
                pedalRoot[i].localRotation = Quaternion.Euler(0, 0, 0);
                pedal[i].localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        //Wheels
        for (int i = 0; i < Wheels.Length; i++)
        {
            Wheels[i].localRotation = Quaternion.Euler(currentRotation, 0, 0);
        }

        //Audio
        float differenceRot = Mathf.Abs(currentRotation - lastSavedRotation);
        if (differenceRot >= 50)
        {
            lastSavedRotation = currentRotation;

            float pitchValue = Mathf.Clamp(speed / 10 + 1, 0, 2.5f);

            wheelSource.pitch = pitchValue;
            wheelSource.volume = 1 - ((pitchValue - 1) / 4);
            wheelSource.PlayOneShot(wheelsSound);
        }



    }


    void OnJump(Vector3 _v)
    {
        //Play jump audio clip;
        source.PlayOneShot(jumpSound, 0.5f);
    }

}

//public static class TransformExtensions
//{
//    public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
//    {
//        var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
//        return localToWorldMatrix.MultiplyPoint3x4(position);
//    }

//    public static Vector3 InverseTransformPointUnscaled(this Transform transform, Vector3 position)
//    {
//        var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
//        return worldToLocalMatrix.MultiplyPoint3x4(position);
//    }
//}
