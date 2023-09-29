using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class LedgeGrab : MonoBehaviour
{

    AdvancedWalkerController advancedWalkerController;
    Mover mover;
    public Transform PlayerRoot;
    public Transform Face;
    public Transform CameraControls;
    public LayerMask IgnoreLayer;

    Vector3 startPoint;
    Vector3 savedStepHeightOffset;
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public bool isActive;
    [HideInInspector] public float lastUngrab;

    void Awake()
    {
        advancedWalkerController = GetComponent<AdvancedWalkerController>();
        mover = GetComponent<Mover>();

        savedStepHeightOffset = new Vector3(0, mover.GetStepHeight() * 3, 0);
    }

    void FixedUpdate()
    {
        if (Time.time < lastUngrab + 1)
        {
            isActive = false;
            return;
        }

        if (isActive)
            startPoint = transform.position + new Vector3(0, -0.05f, 0);
        else
            startPoint = transform.position + savedStepHeightOffset;

        RaycastHit hit;
        if (Physics.Raycast(startPoint, PlayerRoot.forward, out hit, 0.5f, ~IgnoreLayer))
        {
            Debug.DrawRay(startPoint, hit.point - startPoint, Color.yellow);

            Vector3 zPoint = Face.position + PlayerRoot.forward * hit.distance;

            RaycastHit hit_s;
            if (Physics.Raycast(zPoint, Vector3.down, out hit_s, 1f, ~IgnoreLayer))
            {
                Debug.DrawRay(zPoint, hit_s.point - zPoint, Color.yellow);

                isActive = true;
                Vector3 point1 = new Vector3(hit.point.x, hit_s.point.y, hit.point.z);
                Vector3 point2 = hit_s.point;
                Vector3 dir = ((point2 + Vector3.up - (point1 - point2).normalized * 0.15f) - point2).normalized;
                advancedWalkerController.LedgeVector = dir;
                advancedWalkerController.LedgeAngle = Vector3.Dot(CameraControls.forward, PlayerRoot.right);

                Debug.DrawRay(point2, dir, Color.green, 2);
                Debug.DrawRay(point1, Vector3.up, Color.red, 2);
                Debug.DrawRay(point2, Vector3.up, Color.blue, 2);
            }
            else
            {
                isActive = false;
                Debug.DrawRay(zPoint, Vector3.down * 1f, Color.white);
            }
        }
        else
        {
            isActive = false;
            Debug.DrawRay(startPoint, PlayerRoot.forward * 0.5f, Color.white);
        }

    }
}
