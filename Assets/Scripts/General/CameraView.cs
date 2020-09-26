using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform player;
    public float yOffset = 0.8f; // move to the height of the face

    private Gyroscope gyro;
    //to check whether the gyroscope is avaliable on the mobile
    private bool ifGyroEnabled; 

    private GameObject firstView;
    private Quaternion rotMatrix;

    bool EnableGryo(){
        if (SystemInfo.supportsGyroscope){
            gyro = Input.gyro;
            gyro.enabled = true;

            firstView.transform.rotation = Quaternion.Euler(90f, 90f, 0f); //pointing towards
            return true;
        }
        else{
            Debug.Log("no gryoscope is valid in the mobile");
            return false;
        }
    }

    private void Start(){
        firstView = new GameObject("First View");
        firstView.transform.position = transform.position;
        transform.SetParent(firstView.transform);

        ifGyroEnabled = EnableGryo();

        rotMatrix = new Quaternion(0, 0, 1, 0);
    }

    private void Update(){
        //firstView.transform.rotation = player.rotation;
        if (ifGyroEnabled){//to avoid non-reference error
            firstView.transform.position = player.position + new Vector3(0f, yOffset, 0f);
            transform.localRotation = gyro.attitude * rotMatrix;
        }
    }

    
    private void LateUpdate(){
        if (!ifGyroEnabled){
            firstView.transform.position = player.position + new Vector3(0f, yOffset, 0f);
            firstView.transform.rotation = player.rotation;
        }
    }
    
}
