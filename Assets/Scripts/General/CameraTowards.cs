using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTowards : MonoBehaviour
{
    // Start is called before the first frame update
     public Transform player;
     public Vector3 offset = new Vector3(0f, 0.8f, 0.6f);

    void Start(){
        transform.position = player.position + offset;
        transform.LookAt(player.transform.position + new Vector3(0, offset.y, 0));
        transform.SetParent(player);
    }
    /*
    void LateUpdate(){
        transform.position = player.position + offset;
        transform.LookAt(player);
    }*/
}
