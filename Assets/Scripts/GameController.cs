using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class GameController : NetworkBehaviour
{
    public static List<PlayerController> Players = new List<PlayerController>();

    static GameController Singleton;

    public Vector3 lightSpawn = new Vector3(0f, -3f, 0f);
    public Vector3 darkSpawn = new Vector3(0f, 3f, 0f);

    private void Awake()
    {
        Singleton = this;
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Check deaths
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
        Debug.Log("Calling client RPC...");
        Singleton.FinishGameClientRpc();
    }

    [ClientRpc]
    public void DeathClientRpc(ClientRpcParams rpsParams = default)
    {
        StartCoroutine(EndGameSequence());
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
        }
    }

    public static void FinishGame()
    {
        Debug.Log("Calling client RPC...");
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
