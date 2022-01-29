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
    private AnimationState CurrentAnimation { 
        get => currentAnimation_;
        set
        { if (currentAnimation_ != value)
            {
                currentAnimation_ = value;
                switch(value)
                {
                    case AnimationState.Idle:
                        animator.Play("Idle");
                        break;
                    case AnimationState.Air:
                        animator.Play("Jump");
                        break;
                    case AnimationState.Move:
                        animator.Play("Walking");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkDespawn()
    {
        GameObject.Find("Main Camera").transform.SetParent(null);
        NetworkManager.Singleton.Shutdown(true);
        UIController.Singleton.ShowElement("StartScreen");
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
                UIController.Singleton.ShowElement("Notification");
                Time.timeScale = 0f;
            }
        }
    }

    bool TouchesGround()
    {
        Vector2 up = playerType == PlayerType.Light ? Vector2.up : Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * 0.2f, -up, 0.5f, 1);
        if (!hit)
        {
            hit = Physics2D.Raycast(transform.position + Vector3.left * 0.2f, -up, 0.5f, 1);
        }
        return hit;
    }

    void Update()
    {
        bool touchesGround = TouchesGround();

        if (touchesGround)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1)
            {
                CurrentAnimation = AnimationState.Idle;
            }
            else
            {
                CurrentAnimation = AnimationState.Move;
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                sr.flipX = rb.velocity.x < 0;
            }
        }
        else
        {
            CurrentAnimation = AnimationState.Air;
        }

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


            jumpTimer -= Time.deltaTime;

            // Jump
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Space)) && jumpTimer < 0f)
            {
                if (touchesGround)
                {
                    Vector2 up = playerType == PlayerType.Light ? Vector2.up : Vector2.down;
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
    }

    private void FixedUpdate()
    {
        var v = rb.velocity;
        v.x = horizontalControl * horizontalSpeed;
        rb.velocity = v;
    }
}