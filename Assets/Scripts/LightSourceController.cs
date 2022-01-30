using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceController : MonoBehaviour
{
    public EmitLight[] LightSources;
    private PlayerController playerController;


    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerController.playerType == PlayerController.PlayerType.Light)
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
}
