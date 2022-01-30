using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    Rigidbody2D rb;
    float normalGravity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController playerController in playerControllers)
        {
            if (playerController.playerType == PlayerController.PlayerType.Light)
            {
                Vector3 dir = (playerController.transform.position - transform.position).normalized;
                float dist = (playerController.transform.position - transform.position).magnitude;
                RaycastHit2D hit = Physics2D.Raycast(transform.position + dir * transform.localScale.x * 0.75f, dir, dist - 0.75f, 65);
                rb.gravityScale = hit ? -normalGravity : normalGravity;
            }
        }
    }
}
