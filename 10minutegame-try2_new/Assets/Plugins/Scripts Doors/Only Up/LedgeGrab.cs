using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CMF;
using EZCameraShake;

public class LedgeGrab : MonoBehaviour
{

    AdvancedWalkerController advancedWalkerController;
    Mover mover;
    public Transform CameraRoot;
    public Transform CameraControls;
    public LayerMask IgnoreLayer;
    public bool onlyIfForwardMove;

    public AudioSource source;
    public AudioClip[] sound_short;
    public AudioClip[] sound_long;

    public PathType pathSystem = PathType.CatmullRom;

    Vector3 savedStepHeightOffset;
    public Vector3[] waypoints = new Vector3[2];
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public bool isActive;
    [HideInInspector] public GameObject ledgeFollow;
    Tweener ledgeTween;

    void Awake()
    {
        advancedWalkerController = GetComponent<AdvancedWalkerController>();
        mover = GetComponent<Mover>();

        savedStepHeightOffset = new Vector3(0, mover.GetStepHeight() * 3, 0);

        ledgeFollow = new GameObject();
    }

    void FixedUpdate()
    {

        if (isActive)
        {
            //transform.position = ledgeFollow.transform.position;

            if (ledgeTween.ElapsedPercentage() >= 0.6f && (advancedWalkerController.inputY != 0 || advancedWalkerController.inputX != 0))
            {
                KillLedgeGrab();
            }

            if (advancedWalkerController.inputY < 0 && onlyIfForwardMove == false)
            {
                KillLedgeGrab();
            }

            return;
        }



        Vector3 startPoint = transform.position + savedStepHeightOffset;

        RaycastHit hit;
        if (Physics.Raycast(startPoint, CameraRoot.forward, out hit, 0.5f, ~IgnoreLayer))
        {
            Debug.DrawRay(startPoint, hit.point - startPoint, Color.yellow);

            Vector3 zPoint = CameraControls.position + CameraRoot.forward * hit.distance;

            RaycastHit hit_s;
            if (Physics.Raycast(zPoint, Vector3.down, out hit_s, 1f, ~IgnoreLayer))
            {
                Debug.DrawRay(zPoint, hit_s.point - zPoint, Color.yellow);

                if (advancedWalkerController.inputY > 0 || onlyIfForwardMove == true)
                {
                    isActive = true;
                    ledgeFollow.transform.position = transform.position;
                    waypoints[0] = new Vector3(hit.point.x, hit_s.point.y, hit.point.z);
                    waypoints[1] = hit_s.point;
                    dir = Vector3.Cross(waypoints[1] - waypoints[0], Vector3.up).normalized;
                    Debug.DrawRay(waypoints[0], dir, Color.green, 2);
                    Debug.DrawRay(waypoints[0], Vector3.up, Color.red, 2);
                    Debug.DrawRay(waypoints[1], Vector3.up, Color.blue, 2);
                    advancedWalkerController.savedLedgeLR = 0;

                    float speed = Vector3.Distance(transform.position, waypoints[0]) * 0.6f;

                    Debug.Log(speed);

                    //ShakeCamera
                    CameraShaker.Instance.ShakeOnce(speed * 2, 0.5f, 0.5f, speed * 2);

                    if (speed < 0.5f)
                        source.PlayOneShot(sound_short[Random.Range(0, sound_short.Length)], 0.3F);
                    else
                        source.PlayOneShot(sound_long[Random.Range(0, sound_long.Length)], 0.5F);

                    ledgeTween = ledgeFollow.transform.DOPath(waypoints, speed, pathSystem).OnComplete(() => KillLedgeGrab());
                }
            }
            else
            {
                Debug.DrawRay(zPoint, Vector3.down * 1f, Color.white);
            }
        }
        else
        {
            Debug.DrawRay(startPoint, CameraRoot.forward * 0.5f, Color.white);
        }

    }

    void KillLedgeGrab()
    {
        ledgeTween.Kill();
        isActive = false;
    }
}
