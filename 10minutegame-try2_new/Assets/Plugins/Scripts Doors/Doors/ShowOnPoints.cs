using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnPoints : MonoBehaviour
{

    public Mesh mesh;

    void OnDrawGizmos()
    {
        DrawGizmos(transform);
    }

    void DrawGizmos(Transform tr)
    {
        int i = 0;
        foreach (Transform child in tr)
        {
            if (i > 1)
            {
                Gizmos.color = new Color32(0, 255, 0, 128);
                Gizmos.DrawMesh(mesh, child.position, child.rotation, transform.localScale);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, child.position);

            }

            i++;
        }
    }
}
