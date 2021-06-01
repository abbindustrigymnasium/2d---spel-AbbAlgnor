using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;

public class StartMultiplayer : MonoBehaviour
{
    private NetworkManager NetworkManager;
    private
    // Start is called before the first frame update
    void Start()
    {
        int Mode = PlayerPrefs.GetInt("Mode");
        if (Mode == 0)
        {
            StartClient();
        }
        else
        {
            StartHost();
        }
    }
    void StartClient()
    {
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = PlayerPrefs.GetString("IP");
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = PlayerPrefs.GetInt("JoinPort");
        
        Debug.Log("Started Client");
        NetworkManager.Singleton.StartClient();
    }
    public void StartHost()
    {
        NetworkManager.Singleton.GetComponent<UNetTransport>().ServerListenPort = PlayerPrefs.GetInt("HostPort");

        Debug.Log("Started Host");
        NetworkManager.Singleton.StartHost();
    }
}
