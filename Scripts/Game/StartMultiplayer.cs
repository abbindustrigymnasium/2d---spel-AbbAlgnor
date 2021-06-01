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
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = PlayerPrefs.GetString("IP");    //gets the IP the character inputed in the main menu scene
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = PlayerPrefs.GetInt("JoinPort");    //gets the Port the character inputed in the main menu scene
        
        Debug.Log("Started Client");
        NetworkManager.Singleton.StartClient(); //starts the client with the selecter port and ip.
    }
    public void StartHost()
    {
        NetworkManager.Singleton.GetComponent<UNetTransport>().ServerListenPort = PlayerPrefs.GetInt("HostPort");   //gets the Port the character inputed in the main menu scene

        Debug.Log("Started Host");
        NetworkManager.Singleton.StartHost();   //start a host with the selected port. will only work on the local network unless the selected port is port forwared
    }
}
