using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    //string to record the IP address
    public string IPAdressPlayer;
    public string IPAdressMaster;

    public void getPlayerIPAddress(TMP_InputField input){
        Debug.Log("Player: "+input.text);
        IPAdressPlayer = input.text;
    }

    public void getMasterIPAddress(TMP_InputField input){
        Debug.Log("Master: "+input.text);
        IPAdressMaster = input.text;
    }

    public void StartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void QuitGame(){
        Debug.Log("Quit Game");
        Application.Quit();
    }


}
