using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class NetworkController : MonoBehaviour
{
    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void SetIP(string ip)
    {
        if (ip == "")
            ip = "127.0.0.1";

        GetComponent<UNetTransport>().ConnectAddress = ip;
    }

    public void SetPort(string port)
    {
        if (port == "")
            port = "7777";
        GetComponent<UNetTransport>().ConnectPort = int.Parse(port);
        GetComponent<UNetTransport>().ServerListenPort = int.Parse(port);
    }
}
