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

    private class RaycastPoint
    {
        public Vector2 Point;
        public float Angle;
        public RaycastPoint(Vector2 point, float angle)
        {
            Point = point;
            Angle = angle;
        }
    }

    public PolygonInInspector[] Polygons;
    public GameObject TriangleDrawerObject;
    private List<RaycastPoint> raycastPoints;

    private List<Vector2[]> lines_;

    const float Eps = 0.00001f;

    private bool CompareVector2(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.y - b.y) + Mathf.Abs(a.x - b.x) < Eps;
    }

    private bool CompareFloat(float a, float b)
    {
        return Mathf.Abs(a - b) < Eps;
    }

    private class PointLine
    {
        public Vector2 Point;
        public Vector2[] Line;

        public PointLine(Vector2 point, Vector2[] line)
        {
            Point = point;
            Line = line;
        }
    }

    private static float PositiveModulo(float a, float b)
    {
        return a - (a/b) * b;
    }

    List<PointLine> RaycastAngle(List<Vector2[]> lines, RaycastPoint raycastPoint)
    {
        List<PointLine> points = new List<PointLine>();

        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = raycastPoint.Point - pos;
        List<Vector2[]> intersected_lines = new List<Vector2[]>();

        foreach (var line in lines)
        {
            Vector2 lineDirection = line[1] - line[0];
            //Vector2 difference = line[0] - pos;

            float posA = direction.y;
            float posB = -direction.x;
            float posC = posA * pos.x + posB * pos.y;

            float lineA = lineDirection.y;
            float lineB = -lineDirection.x;
            float lineC = lineA * line[0].x + lineB * line[0].y;

            float determinant = posA * lineB - lineA * posB;
            if (determinant == 0)
            {
                continue;
            }
            Vector2 intersection = new Vector2(
                (lineB * posC - posB * lineC) / determinant, 
                (posA * lineC - lineA * posC) / determinant);

            if (intersection.x > Mathf.Min(line[0].x, line[1].x) - Eps && 
                intersection.x < Mathf.Max(line[0].x, line[1].x) + Eps && 
                intersection.y > Mathf.Min(line[0].y, line[1].y) - Eps && 
                intersection.y < Mathf.Max(line[0].y, line[1].y) + Eps && 
                CompareFloat(Mathf.Atan2(direction.y, direction.x) , Mathf.Atan2(intersection.y - pos.y, intersection.x - pos.x)))
            {
                Debug.Log(intersection);
                points.Add(new PointLine(intersection, line));
            }
        }

        points.Sort((PointLine a, PointLine b) => {
            float lenA = (a.Point - pos).sqrMagnitude;
            float lenB = (b.Point - pos).sqrMagnitude;
            if (lenA == lenB)
                return 0;
            return lenA < lenB ? -1 : 1;
        });

        List<PointLine> finalPoints = new List<PointLine>();

        for (int i = 0; i < points.Count; i++)
        {
            PointLine pointLine= points[i];
            finalPoints.Add(pointLine);

            if (!CompareVector2(pointLine.Point, pointLine.Line[0]) && !CompareVector2(pointLine.Point, pointLine.Line[1]))
            {
                return finalPoints;
            }
        }
        return finalPoints;
    }

    void ClearTriangles()
    {
        TriangleDrawer[] objs = FindObjectsOfType<TriangleDrawer>();
        foreach (var obj in objs)
        {
            Destroy(obj.gameObject);
        }
    }

    // Start is called before the first frame update
    void DrawStuff()
    {
        ClearTriangles();

        lines_ = new List<Vector2[]>();
        raycastPoints = new List<RaycastPoint>();
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        foreach (var polygon in Polygons)
        {
            Vector2 previous = polygon.Vertices[polygon.Vertices.Length - 1];
            foreach (var vertex in polygon.Vertices)
            {
                lines_.Add(new Vector2[] { new Vector2(previous.x, previous.y), new Vector2(vertex.x, vertex.y) });
                previous = vertex;
                raycastPoints.Add(new RaycastPoint(vertex, Mathf.Atan2(vertex.y - pos.y, vertex.x - pos.x)));
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
        raycastPoints.Sort((RaycastPoint a, RaycastPoint b) => {
            if (a.Angle == b.Angle)
                return 0;
            return a.Angle < b.Angle ? -1 : 1;
        });

        List<List<PointLine>> endPoints = new List<List<PointLine>>();

        foreach (var point in raycastPoints)
        {
            List<PointLine> newPoints = RaycastAngle(lines_, point);
            endPoints.Add(newPoints);
        }

        //Debug.Log("End point rays: " + endPoints.Count);
        for (int i = 0; i < endPoints.Count; i++)
        {
            var points = endPoints[i];
            var nextPoints = endPoints[(i + 1) % endPoints.Count];

            //Debug.Log("Points on ray: " + points.Count + ", next: " + nextPoints.Count);
            foreach (var point in points)
            {
                foreach (var nextPoint in nextPoints)
                {
                    //Debug.Log(Mathf.Atan2(point.Point.y - pos.y, point.Point.x - pos.x));
                    if (point.Line[0].x == nextPoint.Line[0].x && point.Line[0].y == nextPoint.Line[0].y && point.Line[1].x == nextPoint.Line[1].x && point.Line[1].y == nextPoint.Line[1].y)
                    {
                        if (CompareFloat(Mathf.Atan2(point.Point.y - pos.y, point.Point.x - pos.x), Mathf.Atan2(nextPoint.Point.y - pos.y, nextPoint.Point.x - pos.x)))
                        {
                            continue;
                        }
                        GameObject triangleObject = Instantiate(TriangleDrawerObject);
                        TriangleDrawer drawer;
                        if (triangleObject.TryGetComponent<TriangleDrawer>(out drawer))
                        {
                            drawer.DrawTriangle(pos, new Vector2(point.Point.x, point.Point.y), new Vector2(nextPoint.Point.x, nextPoint.Point.y), new Color(1.0f, 0.05f * i, 0, 0.5f));
                        }
                    }
                }
            }
        }

        /*
        foreach(var line in lines_)
        {
            GameObject triangleObject = Instantiate(TriangleDrawerObject);
            TriangleDrawer drawer;
            if (triangleObject.TryGetComponent<TriangleDrawer>(out drawer))
            {
                drawer.DrawTriangle(new Vector2(0, 0), new Vector2(line[0].x, line[0].y), new Vector2(line[1].x, line[1].y), new Color(1, 0, 0, 0.5f));
            }
        }
        */
    }

    private void Update()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position = transform.position + dir * Time.deltaTime * 3;

        DrawStuff();
    }
}
