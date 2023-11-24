using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ExtrudeSprite : MonoBehaviour
{

    public Transform destructible_2d_Object;
    public Transform colliderObject;

    public Color extrudeColor = Color.white;
    PolygonCollider2D[] pol;
    public float wallThickness;

    public float rotationAngles;


    List<Mesh> meshes = new List<Mesh>();

    void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        meshes.Clear();
        transform.eulerAngles = Vector3.zero;

        pol = colliderObject.GetComponentsInChildren<PolygonCollider2D>();

        for (int c = 0; c < pol.Length; c++)
        {
            //Face mesh
            Mesh meshFace = pol[c].CreateMesh(true, false);
            meshFace.RecalculateNormals();
            meshes.Add(meshFace);
            //Back mesh
            Mesh meshBack = pol[c].CreateMesh(true, false);
            meshBack.RecalculateNormals();
            meshBack.triangles = meshBack.triangles.Reverse().ToArray();
            meshBack.name = "Back";
            meshes.Add(meshBack);


            //Around mesh
            List<Vector2> p2 = new List<Vector2>();
            float scaleX = colliderObject.localScale.x * destructible_2d_Object.localScale.x;
            float scaleY = colliderObject.localScale.y * destructible_2d_Object.localScale.y;
            Vector2 offset = new Vector2(colliderObject.position.x, colliderObject.position.y);
            Mesh m = new Mesh();

            for (int j = 0; j < pol[c].pathCount; j++)
            {
                for (int i = 0; i < pol[c].GetPath(j).Length; i++)
                {
                    Vector2 newV2 = new Vector2(pol[c].GetPath(j)[i].x * scaleX, pol[c].GetPath(j)[i].y * scaleY) + offset;


                    p2.Add(newV2);
                }


                m = CreateMesh(p2.ToArray(), true, wallThickness);
                meshes.Add(m);

                p2.Clear();
            }
        }


        //Combine
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        float zOffset = 0;
        for (int i = 0; i < meshes.Count; i++)
        {
            if (meshes[i].name != "")
            {
                if (meshes[i].name == "Back")
                    zOffset = wallThickness;
                else
                    zOffset = -wallThickness;
            }
            else
            {
                zOffset = 0;
            }


            combine[i].mesh = meshes[i];
            combine[i].transform = Matrix4x4.TRS(transform.InverseTransformPoint(new Vector3(0, 0, zOffset)) + (transform.position - destructible_2d_Object.position),
                Quaternion.Inverse(transform.rotation), Vector3.one);
            //combine[i].transform = transform.localToWorldMatrix;
        }

        //Create mesh
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.eulerAngles = new Vector3(0, rotationAngles, 0);
        GetComponent<MeshRenderer>().material.color = extrudeColor;
    }

    private static Mesh CreateMesh(Vector2[] poly, bool isSide, float wallThickness = 0.25f)
    {
        // convert polygon to triangles
        Triangulator triangulator = new Triangulator(poly);
        int[] tris = triangulator.Triangulate();
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[poly.Length * 2];

        for (int i = 0; i < poly.Length; i++)
        {
            vertices[i].x = poly[i].x;
            vertices[i].y = poly[i].y;
            vertices[i].z = -wallThickness; // front vertex
            vertices[i + poly.Length].x = poly[i].x;
            vertices[i + poly.Length].y = poly[i].y;
            vertices[i + poly.Length].z = wallThickness;  // back vertex    
        }
        int[] triangles = new int[tris.Length * 2 + poly.Length * 6];
        int count_tris = 0;

        if (isSide == false)
        {
            for (int i = 0; i < tris.Length; i += 3)
            {
                triangles[i + 2] = tris[i + 2];
                triangles[i + 1] = tris[i + 1];
                triangles[i] = tris[i];
            } // front vertices

            count_tris += tris.Length;
            for (int i = 0; i < tris.Length; i += 3)
            {
                triangles[count_tris + i + 2] = tris[i] + poly.Length;
                triangles[count_tris + i + 1] = tris[i + 1] + poly.Length;
                triangles[count_tris + i] = tris[i + 2] + poly.Length;
            } // back vertices
        }


        count_tris += tris.Length;

        if (isSide == true)
        {
            for (int i = 0; i < poly.Length; i++)
            {
                // triangles around the perimeter of the object
                int n = (i + 1) % poly.Length;

                triangles[count_tris] = i + poly.Length;
                triangles[count_tris + 1] = n + poly.Length;
                triangles[count_tris + 2] = n;
                triangles[count_tris + 3] = i + poly.Length;
                triangles[count_tris + 4] = n;
                triangles[count_tris + 5] = i;

                count_tris += 6;
            }
        }


        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        //m.RecalculateBounds();
        //m.Optimize();
        return m;
    }
}

/// <summary>
/// 
/// </summary>
/// <remarks>Source: http://wiki.unity3d.com/index.php?title=Triangulator </remarks>
public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}

