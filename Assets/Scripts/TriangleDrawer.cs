using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleDrawer : MonoBehaviour
{
    private GameObject m_goTriangle;
    private Mesh m_meshTriangle;

    bool started = false;

    public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color col)
    {
        Start();

        m_goTriangle.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default")) { color = col };
        m_meshTriangle.vertices = new Vector3[] { new Vector3(p1.x, p1.y, 0), new Vector3(p2.x, p2.y, 0), new Vector3(p3.x, p3.y, 0) };
        m_meshTriangle.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1f), new Vector2(1f, 2f) };
        m_meshTriangle.triangles = new int[] { 0, 1, 2 };
    }

    // Start is called before the first frame update
    void Start()
    {
        if (started)
            return;

        m_goTriangle = gameObject;
        m_goTriangle.AddComponent<MeshFilter>();
        m_goTriangle.AddComponent<MeshRenderer>();

        m_meshTriangle = m_goTriangle.GetComponent<MeshFilter>().mesh;
        m_meshTriangle.Clear();

        started = true;
    }
}
