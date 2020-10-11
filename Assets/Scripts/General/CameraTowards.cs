using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTowards : MonoBehaviour
{
    // Start is called before the first frame update
     public Transform player;
     public Vector3 offset = new Vector3(0f, 0.6f, 1.2f);

    void Start(){
        transform.position = player.position + offset;
        transform.LookAt(player);
        transform.SetParent(player);
    }
    /*
    void LateUpdate(){
        transform.position = player.position + offset;
        transform.LookAt(player);
    }*/
}
