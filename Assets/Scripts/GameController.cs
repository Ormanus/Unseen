using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class GameController : NetworkBehaviour
{
    public static List<PlayerController> Players = new List<PlayerController>();

    static GameController Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public static void ReturnToMain()
    {
        Debug.Log("Returning to the main menu...");
        GameObject.Find("Main Camera").transform.SetParent(null);
        NetworkManager.Singleton.Shutdown(true);
        UIController.Singleton.ShowElement("StartScreen");
        Players.Clear();
    }

    public static void FinishGame()
    {
        Debug.Log("Calling client RPC...");
        Singleton.FinishGameClientRpc();
    }

    [ClientRpc]
    public void FinishGameClientRpc()
    {
        Debug.Log("Ending the game...");
        NetworkManager.Singleton.Shutdown();
        UIController.Singleton.ShowElement("EndScreen");
    }
}
