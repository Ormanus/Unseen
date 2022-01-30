using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerController : NetworkBehaviour
{
    public enum PlayerType
    {
        Light,
        Dark
    }
    const float jumpForce = 8f;
    const float horizontalSpeed = 5f;

    float horizontalControl = 0;
    Rigidbody2D rb;
    float jumpTimer = 0f;

    public PlayerType playerType {get; private set;}

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GameController.Players.Add(this);
    }

    public override void OnNetworkDespawn()
    {
        GameController.ReturnToMain();
    }

    public bool PublicIsHost
    {
        get => IsHost;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (IsHost)
            {
                rb.position = Vector2.up * 3f;
                playerType = PlayerType.Light;
                Debug.Log("Started the game as the light character");
            }
            else
            {
                rb.position = Vector2.down * 3f;
                rb.gravityScale = -rb.gravityScale;
                playerType = PlayerType.Dark;
                Debug.Log("Started the game as the dark character");
            }
            var cam = GameObject.Find("Main Camera");
            if (cam)
            {
                cam.transform.SetParent(transform);
                cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            }
        }

        if (IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count > 1)
            {
                Time.timeScale = 1f;
                UIController.Singleton.HideAll();
            }
            else
            {
                UIController.Singleton.ShowElement("Waiting");
                Time.timeScale = 0f;
            }
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            PollControls();
        }

        if (IsServer)
        {
            CheckDeaths();
        }
    }

    private void CheckDeaths()
    {
        foreach (var player in GameController.Players)
        {
            
        }
    }

    private void PollControls()
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
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Space)) && jumpTimer < 0f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * 0.2f, -up, 0.5f, 1);
            if (hit)
            {
                rb.AddForce(up * jumpForce, ForceMode2D.Impulse);
                jumpTimer = 0.1f;
            }
            else
            {
                hit = Physics2D.Raycast(transform.position + Vector3.left * 0.2f, -up, 0.5f, 1);
                if (hit)
                {
                    rb.AddForce(up * jumpForce, ForceMode2D.Impulse);
                    jumpTimer = 0.1f;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && IsServer)
        {
            Time.timeScale = 1f;
            UIController.Singleton.HideAll();
        }
    }

    private void FixedUpdate()
    {
        var v = rb.velocity;
        v.x = horizontalControl * horizontalSpeed;
        rb.velocity = v;
    }
}