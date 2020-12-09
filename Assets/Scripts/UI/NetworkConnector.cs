using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkConnector : MonoBehaviour
{
    public NetworkManager manager;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("isMaster") == 1){
            manager.StartHost();
        }
        else{
            manager.networkAddress = PlayerPrefs.GetString("IPAdressPlayer");
            manager.StartClient();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
