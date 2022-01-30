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
    private Animator animator;

    enum AnimationState
    {
        Idle,
        Air,
        Move
    }

    private AnimationState currentAnimation_;

    private void SetAnimation(AnimationState animation)
    {
        if (currentAnimation_ != animation)
        {
            string prefix = playerType == PlayerType.Light ? "" : "Night";
            currentAnimation_ = animation;
            switch (animation)
            {
                case AnimationState.Idle:
                    animator.Play(prefix + "Idle");
                    break;
                case AnimationState.Air:
                    animator.Play(prefix + "Jump");
                    break;
                case AnimationState.Move:
                    animator.Play(prefix + "Walking");
                    break;
                default:
                    break;
            }
        }
    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GameController.Players.Add(this);
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkDespawn()
    {
        GameController.ReturnToMain();
    }

    public bool PublicIsHost
    {
        get => IsServer;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (IsHost)
            {
                rb.position = GameController.lightSpawn;
                playerType = PlayerType.Light;
                Debug.Log("Started the game as the light character");
            }
            else
            {
                rb.position = GameController.darkSpawn;
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
        else
        {
            playerType = IsHost ? PlayerType.Dark : PlayerType.Light;
        }

        if (IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count > 1)
            {
                Time.timeScale = 1f;
                UIController.Singleton.HideAll();
                GameController.respawning = false;
            }
            else
            {
                UIController.Singleton.ShowElement("Waiting");
                Time.timeScale = 0f;
            }
        }
    }

    bool TouchesGround()
    {
        Vector2 up = playerType == PlayerType.Light ? Vector2.up : Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * 0.2f, -up, 0.5f, 65);
        if (!hit)
        {
            hit = Physics2D.Raycast(transform.position + Vector3.left * 0.2f, -up, 0.5f, 1);
        }
        return hit;
    }

    void Update()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.flipY = playerType == PlayerType.Dark;

        BoxCollider2D boxCollider2D= sr.GetComponent<BoxCollider2D>();
        boxCollider2D.offset = playerType == PlayerType.Light ? new Vector2(0, -0.12f) : new Vector2(0, 0.15f);

        bool touchesGround = TouchesGround();

        if (touchesGround)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1)
            {
                SetAnimation(AnimationState.Idle);
            }
            else
            {
                SetAnimation(AnimationState.Move);
                sr = gameObject.GetComponent<SpriteRenderer>();
                sr.flipX = rb.velocity.x > 0;
            }
        }
        else
        {
            SetAnimation(AnimationState.Air);
        }

        if (IsOwner)
        {
            PollControls();
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
            if (TouchesGround())
            {
                rb.AddForce(up * jumpForce, ForceMode2D.Impulse);
                jumpTimer = 0.1f;
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