using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalLightMesh
{
    public static List<Vector2[]> Triangles = new List<Vector2[]>();

    private static ComputeBuffer previousBuffer;
    public static void Clear()
    {
        Triangles.Clear();
    }
    public static void AddTriangle(Vector2[] triangle)
    {
        Triangles.Add(triangle);
    }
    const int MaxPointCount = 4096;
    public static void ApplyToShader(Material material)
    {
        List<Vector2> vectors = new List<Vector2>();
        int count = 0;
        foreach (var triangle in Triangles)
        {
            for (int i = 0; i < 3 && count < MaxPointCount;  i++, count++)
            {
                vectors.Add(new Vector2(triangle[i].x, triangle[i].y));
            }
        }

        if (previousBuffer!=null)
        {
            previousBuffer.Release();
        }

        ComputeBuffer buffer;
        buffer = new ComputeBuffer(vectors.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector2)), ComputeBufferType.Default);
        buffer.SetData(vectors);

        material.SetBuffer("_LightMeshVectors", buffer);
        //material.SetVectorArray("_LightMeshVectors", trianglePoints);
        material.SetInteger("_LightMeshVectorsLength", vectors.Count);
        previousBuffer = buffer;
    }
}
