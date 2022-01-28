using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleDrawer : MonoBehaviour
{
    public Vector2[] Vertices;
    private GameObject m_goTriangle;
    private Mesh m_meshTriangle;
    private void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        m_meshTriangle.vertices = new Vector3[] { new Vector3(p1.x, p1.y, 0), new Vector3(p2.x, p2.y, 0), new Vector3(p3.x, p3.y, 0) };
        m_meshTriangle.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1f), new Vector2(1f, 2f) };
        m_meshTriangle.triangles = new int[] { 0, 1, 2 };
    }

    // Start is called before the first frame update
    void Start()
    {
        m_goTriangle = this.gameObject;
        m_goTriangle.AddComponent<MeshFilter>();
        m_goTriangle.AddComponent<MeshRenderer>();
        m_goTriangle.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };

        m_meshTriangle = m_goTriangle.GetComponent<MeshFilter>().mesh;
        m_meshTriangle.Clear();

        DrawTriangle(new Vector2(Vertices[0].x, Vertices[0].y), new Vector2(Vertices[1].x, Vertices[1].y), new Vector2(Vertices[2].x, Vertices[2].y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
