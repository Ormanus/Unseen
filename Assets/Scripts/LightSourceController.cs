using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceController : MonoBehaviour
{
    public EmitLight[] LightSources;

    // Update is called once per frame
    void Update()
    {
        GlobalLightMesh.Clear();
        foreach (var lightSource in LightSources)
        {
            lightSource.DrawStuff();
        }
        GameObject bgSquare = GameObject.Find("Background Square");
        Material mat = bgSquare.GetComponent<SpriteRenderer>().material;
        GlobalLightMesh.ApplyToShader(mat);
    }
}
