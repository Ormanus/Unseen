using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceController : MonoBehaviour
{
    public EmitLight[] LightSources;
    void ClearTriangles()
    {
        TriangleDrawer[] objs = FindObjectsOfType<TriangleDrawer>();
        foreach (var obj in objs)
        {
            Destroy(obj.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ClearTriangles();
        foreach (var lightSource in LightSources)
        {
            lightSource.DrawStuff();
        }
    }
}
