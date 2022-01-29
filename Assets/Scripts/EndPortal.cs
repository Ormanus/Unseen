using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EndPortal : MonoBehaviour
{
    public PlayerController.PlayerType playerType;

    static bool lightReached = false;
    static bool nightReached = false;

    private void Start()
    {
        var portals = FindObjectsOfType<EndPortal>();

        if (portals.Length < 2)
        {
            Debug.LogWarning("Too few end portals!");
        }

        bool oppositeFound = false;
        foreach (EndPortal portal in portals)
        {
            if (portal.playerType != playerType)
            {
                oppositeFound = true;
                break;
            }
        }

        if (!oppositeFound)
        {
            Debug.LogWarning("Only one type of end portal found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.playerType == playerType)
            {
                if (playerType == PlayerController.PlayerType.Light)
                {
                    lightReached = true;
                }
                else
                {
                    nightReached = true;
                }

                if (lightReached && nightReached)
                {
                    UIController.Singleton.ShowElement("EndScreen");
                    NetworkManager.Singleton.Shutdown();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.playerType == playerType)
            {
                if (playerType == PlayerController.PlayerType.Light)
                {
                    lightReached = false;
                }
                else
                {
                    nightReached = false;
                }
            }
        }
    }
}