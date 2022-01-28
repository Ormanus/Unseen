using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDrawer : MonoBehaviour
{
    [System.Serializable]
    public struct PolygonInInspector
    {
        public Vector2[] Vertices;
    }

    public PolygonInInspector[] Polygons;
    public GameObject TriangleDrawerObject;

    private List<Vector2[]> lines_;
    // Start is called before the first frame update
    void Start()
    {
        lines_ = new List<Vector2[]>();

        foreach (var polygon in Polygons)
        {
            Vector2 previous = polygon.Vertices[polygon.Vertices.Length - 1];
            foreach (var vertex in polygon.Vertices)
            {
                lines_.Add(new Vector2[] { new Vector2(previous.x, previous.y), new Vector2(vertex.x, vertex.y) });
                previous = vertex;
            }

            // Draw the polygon
            for (int i = 2; i <  polygon.Vertices.Length; i++)
            {
                GameObject triangleObject = Instantiate(TriangleDrawerObject);
                TriangleDrawer drawer;
                if (triangleObject.TryGetComponent<TriangleDrawer>(out drawer))
                {
                    drawer.DrawTriangle(new Vector2(polygon.Vertices[0].x, polygon.Vertices[0].y), new Vector2(polygon.Vertices[i-1].x, polygon.Vertices[i-1].y), new Vector2(polygon.Vertices[i].x, polygon.Vertices[i].y), new Color(1, 1, 1, 0.75f));
                }
            }
        }

        foreach(var line in lines_)
        {
            GameObject triangleObject = Instantiate(TriangleDrawerObject);
            TriangleDrawer drawer;
            if (triangleObject.TryGetComponent<TriangleDrawer>(out drawer))
            {
                drawer.DrawTriangle(new Vector2(0, 0), new Vector2(line[0].x, line[0].y), new Vector2(line[1].x, line[1].y), new Color(1, 0, 0, 0.5f));
            }
        }
    }
}
