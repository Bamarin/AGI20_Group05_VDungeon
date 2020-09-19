using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform player;

    void LateUpdate(){
        transform.position = player.position;
        transform.rotation = player.rotation;
    }
    
}
