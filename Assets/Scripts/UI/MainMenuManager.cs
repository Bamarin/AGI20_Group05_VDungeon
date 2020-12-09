using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Text IPv4AdressText;
    public TMP_InputField playerInputIPAddress;

    //PlayerPrefs is used to transfer data from main menu to the game scene
    public void setPlayerIPAddress(TMP_InputField input){
        Debug.Log("Player: "+input.text);
        PlayerPrefs.SetString("IPAdressPlayer", input.text);
        PlayerPrefs.SetInt("isMaster", 0);
    }

    public void setMasterIPAddress(){
        Debug.Log("Master");
        PlayerPrefs.SetInt("isMaster", 1);
    }

    public void StartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void QuitGame(){
        Debug.Log("Quit Game");
        Application.Quit();
    }

    //reference: https://answers.unity.com/questions/1731994/get-the-device-ip-address-from-unity.html
    //IPv4
    public string GetLocalIPAddress()
     {
         var host = Dns.GetHostEntry(Dns.GetHostName());
         foreach (var ip in host.AddressList)
         {
             if (ip.AddressFamily == AddressFamily.InterNetwork)
             {
                 return ip.ToString();
             }
         }
         throw new System.Exception("No network adapters with an IPv4 address in the system!");
     }

    public void setPlayerIPAddressInputField(TMP_InputField input){
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)){
            setPlayerIPAddress(input);
            StartGame();
        }
    }

    public void setPlayerIPAddressButton(TMP_InputField input){
        setPlayerIPAddress(input);
        StartGame();
    }


    void Start(){
        PlayerPrefs.SetInt("isMaster", -1);
        IPv4AdressText.text = GetLocalIPAddress();
    }

    void Update()
    {
        
    }
}
