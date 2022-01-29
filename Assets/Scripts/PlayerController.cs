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
    const float jumpForce = 200f;
    const float horizontalSpeed = 5f;

    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

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
            
        }
    }

    void Update()
    {
        transform.position = Position.Value;


        if (IsOwner)
        {
            // Horizontal controls
            horizontalControl = 0;
            if (Input.GetKey(KeyCode.D))
            {
                horizontalControl += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                horizontalControl -= 1;
            }

            Vector2 up = playerType == PlayerType.Light ? Vector2.up : Vector2.down;

            jumpTimer -= Time.deltaTime;

            // Jump
            if (Input.GetKeyDown(KeyCode.W) && jumpTimer < 0f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, -up, 1f, 1);
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

        SendUpdatedPositionServerRpc();
    }

    [ServerRpc]
    void SendUpdatedPositionServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = transform.position;
    }
}