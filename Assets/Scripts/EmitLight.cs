using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitLight : MonoBehaviour
{
    private List<BlockLight> walls_;
    public GameObject TriangleDrawerObject;

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

    List<PointLine> RaycastAngle(List<Vector2[]> lines, RaycastPoint raycastPoint, Vector2 offset)
    {
        List<PointLine> points = new List<PointLine>();

        Vector2 pos = new Vector2(transform.position.x, transform.position.y) + offset;
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
                CompareFloat(Mathf.Atan2(direction.y, direction.x), Mathf.Atan2(intersection.y - pos.y, intersection.x - pos.x)))
            {
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
            PointLine pointLine = points[i];
            finalPoints.Add(pointLine);

            if (!CompareVector2(pointLine.Point, pointLine.Line[0]) && !CompareVector2(pointLine.Point, pointLine.Line[1]))
            {
                return finalPoints;
            }
        }
        return finalPoints;
    }

    // Start is called before the first frame update
    public void DrawStuff()
    {
        lines_ = new List<Vector2[]>();
        raycastPoints = new List<RaycastPoint>();
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        foreach (var wall in walls_)
        {
            BoxCollider2D collider = wall.gameObject.GetComponent<BoxCollider2D>();
            Vector2 boxPosition = collider.transform.position;
            Vector2 size = collider.transform.localScale;
            Vector2[] vertices = new Vector2[] { boxPosition + size / 2, boxPosition + new Vector2(size.x, -size.y) / 2, boxPosition - size / 2, boxPosition + new Vector2(-size.x, size.y) / 2 };

            Vector2 previous = vertices[vertices.Length - 1];
            foreach (var vertex in vertices)
            {
                lines_.Add(new Vector2[] { new Vector2(previous.x, previous.y), new Vector2(vertex.x, vertex.y) });
                previous = vertex;
                raycastPoints.Add(new RaycastPoint(vertex, Mathf.Atan2(vertex.y - pos.y, vertex.x - pos.x)));
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
            endPoints.Add(RaycastAngle(lines_, point, new Vector2(0, 0)));
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
                        triangleObject.transform.position += new Vector3(0, 0, 1);
                        TriangleDrawer drawer;
                        if (triangleObject.TryGetComponent<TriangleDrawer>(out drawer))
                        {
                            drawer.DrawTriangle(pos, new Vector2(point.Point.x, point.Point.y), new Vector2(nextPoint.Point.x, nextPoint.Point.y), new Color(1f, 1f, 0));
                        }
                    }
                }
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        walls_ = new List<BlockLight>();

        BlockLight[] realWalls = FindObjectsOfType<BlockLight>();
        GameObject emptyObject = new GameObject("wall");
        emptyObject.AddComponent<BlockLight>();
        emptyObject.AddComponent<BoxCollider2D>();

        GameObject upperWall = Instantiate(emptyObject);
        upperWall.transform.position = new Vector3(0, 51, 0);
        upperWall.transform.localScale = new Vector3(100, 2, 1);

        GameObject rightWall = Instantiate(emptyObject);
        rightWall.transform.position = new Vector3(51, 0, 0);
        rightWall.transform.localScale = new Vector3(2, 100, 1);

        GameObject lowerWall = Instantiate(emptyObject);
        lowerWall.transform.position = new Vector3(0, -51, 0);
        lowerWall.transform.localScale = new Vector3(100, 2, 1);

        GameObject leftWall = Instantiate(emptyObject);
        leftWall.transform.position = new Vector3(-51, 0, 0);
        leftWall.transform.localScale = new Vector3(2, 100, 1);

        walls_.Add(upperWall.GetComponent<BlockLight>());
        walls_.Add(rightWall.GetComponent<BlockLight>());
        walls_.Add(lowerWall.GetComponent<BlockLight>());
        walls_.Add(leftWall.GetComponent<BlockLight>());

        foreach (var realWall in realWalls)
        {
            walls_.Add(realWall);
        }
    }
}
