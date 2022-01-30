using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class GameController : NetworkBehaviour
{
    public static List<PlayerController> Players = new List<PlayerController>();

    static GameController Singleton;

    public static Vector3 lightSpawn = new Vector3(0f, 3f, 0f);
    public static Vector3 darkSpawn = new Vector3(0f, -3f, 0f);

    public static bool respawning = true;

    private void Awake()
    {
        Singleton = this;
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer && !respawning)
        {
            if (Players.Count == 2)
            {
                Vector2 pos1 = Players[0].transform.position;
                Vector2 pos2 = Players[1].transform.position;

                Vector2 delta = pos2 - pos1;
                Vector2 ignoreRadius = delta.normalized * 0.6f;

                RaycastHit2D hit = Physics2D.Raycast(pos1 + ignoreRadius, delta, delta.magnitude, 65 + 8);

                if (hit)
                {
                    var player = hit.collider.GetComponent<PlayerController>();
                    if (player)
                    {
                        if (player == Players[0])
                        {
                            Debug.Log("ERROR!!");
                        }
                        else
                        {
                            Debug.Log("HIT!");
                            Death();
                        }
                    }
                }
            }
        }
    }

    public static void ReturnToMain()
    {
        Debug.Log("Returning to the main menu...");
        GameObject.Find("Main Camera").transform.SetParent(null);
        NetworkManager.Singleton.Shutdown(true);
        UIController.Singleton.ShowElement("StartScreen");
        Players.Clear();
    }

    public static void Death()
    {
        Debug.Log("Calling death RPC...");
        Singleton.DeathClientRpc();
    }

    [ClientRpc]
    public void DeathClientRpc(ClientRpcParams rpsParams = default)
    {
        respawning = true;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        GameObject.Find("Main Camera").transform.SetParent(null);
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 3f;
            foreach (var player in Players)
            {
                player.transform.localScale = Vector3.one * t;
            }
            yield return null;
        }
        Debug.Log("Respawning...");
        foreach (var player in Players)
        {
            player.transform.localScale = Vector3.one;
            player.transform.position = player.playerType == PlayerController.PlayerType.Light ? lightSpawn : darkSpawn;

            if (player.IsLocalPlayer)
            {
                var cam = GameObject.Find("Main Camera");
                if (cam)
                {
                    cam.transform.SetParent(player.transform);
                    cam.transform.localPosition = new Vector3(0f, 0f, -10f);
                }
            }
            respawning = false;
        }
    }

    public static void FinishGame()
    {
        Debug.Log("Calling end RPC...");
        Singleton.FinishGameClientRpc();
    }

    [ClientRpc]
    public void FinishGameClientRpc(ClientRpcParams rpsParams = default)
    {
        StartCoroutine(EndGameSequence());
    }

    IEnumerator EndGameSequence()
    {
        GameObject.Find("Main Camera").transform.SetParent(null);
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 1f;
            foreach (var player in Players)
            {
                player.transform.localScale = Vector3.one * t;
            }
            yield return null;
        }
        Debug.Log("Ending the game...");
        NetworkManager.Singleton.Shutdown();
        UIController.Singleton.ShowElement("EndScreen");
    }
}
