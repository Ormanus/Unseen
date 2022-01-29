using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public enum PlayerType
    {
        Light,
        Dark
    }
    const float jumpForce = 7f;
    const float horizontalSpeed = 7f;

    float horizontalControl = 0;
    Rigidbody2D rb;
    float jumpTimer = 0f;

    public PlayerType playerType {get; private set;}

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (IsServer)
            {
                rb.position = Vector2.up * 3f;
                playerType = PlayerType.Light;
            }
            else
            {
                rb.position = Vector2.down * 3f;
                rb.gravityScale = -rb.gravityScale;
                playerType = PlayerType.Dark;
            }

            GameObject.Find("Main Camera").transform.SetParent(transform);
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            // Horizontal controls
            horizontalControl = 0;
            horizontalControl = Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyCode.D))
            {
                horizontalControl += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                horizontalControl -= 1;
            }

            horizontalControl = Mathf.Clamp(horizontalControl, -1, 1);

            Vector2 up = playerType == PlayerType.Light ? Vector2.up : Vector2.down;

            jumpTimer -= Time.deltaTime;

            // Jump
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && jumpTimer < 0f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, -up, 0.20f, 1);
                if (hit)
                {
                    rb.AddForce(up * jumpForce, ForceMode2D.Impulse);
                    jumpTimer = 0.5f;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        var v = rb.velocity;
        v.x = horizontalControl * horizontalSpeed;
        rb.velocity = v;
    }
}