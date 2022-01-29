using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalLightMesh
{
    public static List<Vector2[]> Triangles = new List<Vector2[]>();
    public static void Clear()
    {
        Triangles.Clear();
    }
    public static void AddTriangle(Vector2[] triangle)
    {
        Triangles.Add(triangle);
    }
    const int MaxPointCount = 1023;
    public static void ApplyToShader(Material material)
    {
        List<Vector4> vectors = new List<Vector4>();
        int count = 0;
        foreach (var triangle in Triangles)
        {
            for (int i = 0; i < 3 && count < MaxPointCount;  i++, count++)
            {
                vectors.Add(new Vector4(triangle[i].x, triangle[i].y, 0, 0));
            }
        }

        Vector4[] trianglePoints = new Vector4[MaxPointCount];

        vectors.CopyTo(trianglePoints);

        material.SetVectorArray("_LightMeshVectors", trianglePoints);
        material.SetInteger("_LightMeshVectorsLength", vectors.Count);
    }
}
