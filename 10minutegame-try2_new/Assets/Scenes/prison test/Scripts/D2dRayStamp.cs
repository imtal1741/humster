using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CW.Common;
using Destructible2D;

public class D2dRayStamp : MonoBehaviour
{
    public D2dDestructible.PaintType Paint;
    public D2dShape StampShape;
    public Color Color = Color.white;
    public Vector2 Size = Vector2.one;
    public float ScaleMin = 0.75f;
    public float ScaleMax = 1.25f;
    public float Angle;
    public float TwistMin = 0.0f;
    public float TwistMax = 360.0f;
    public LayerMask Layers = -1;
    public D2dDestructible Exclude;
    public float distance = 2f;

    ExtrudeSprite lastExtrudeSprite;


    [ContextMenu("Generate")]
    private void DoStamp()
    {
        var position = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distance))
        {
            if (hit.transform.gameObject.GetComponent<ExtrudeSprite>() == null)
                return;

            lastExtrudeSprite = hit.transform.GetComponent<ExtrudeSprite>();
            //position = RotatePointAroundPoint(hit.transform.position, hit.point, 45) + new Vector3(0, lastExtrudeSprite.destructible_2d_Object.localPosition.y, 0);
            position = (45 * (hit.point - hit.transform.position) + hit.transform.position);

            Debug.DrawLine(transform.position, position, Color.yellow);
        }
        else
        {
            return;
        }

        var scale = Random.Range(ScaleMin, ScaleMax);
        var twist = Random.Range(TwistMin, TwistMax);

        D2dStamp.All(Paint, position, Size * scale, Angle + twist, StampShape, Color, Layers, Exclude);
    }


    Vector3 RotatePointAroundPoint(Vector3 point1, Vector3 point2, float angle)
    {
        angle *= Mathf.Deg2Rad;
        var x = Mathf.Cos(angle) * (point1.x - point2.x) - Mathf.Sin(angle) * (point1.y - point2.y) + point2.x;
        var y = Mathf.Sin(angle) * (point1.x - point2.x) + Mathf.Cos(angle) * (point1.y - point2.y) + point2.y;
        return new Vector3(x, y);
    }
}
