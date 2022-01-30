using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    Vector2 previousScale;

    // Based on the top response of https://answers.unity.com/questions/620699/scaling-my-background-sprite-to-fill-screen-2d-1.html
    void Resize()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        if ((float)Screen.height / (float)Screen.width > height / width)
        {
            transform.localScale = new Vector3(worldScreenHeight / height, worldScreenHeight / height, 0);
        }
        else
        {
            transform.localScale = new Vector3(worldScreenWidth / width, worldScreenWidth / width, 0);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        previousScale = new Vector2(Screen.width, Screen.height);
        Resize();
    }


    // Update is called once per frame
    void Update()
    {
        if (Screen.width != previousScale.x || Screen.height != previousScale.y)
        {
            previousScale = new Vector2(Screen.width, Screen.height);
            Resize();
        }
    }
}
