using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //keep the offset from the the object center to the mouse clicked position
    public Vector3 offset;
    //the plane where the object is moving on
    public Plane movePlane;

    // when the mouse is clicked on a collider
    void OnMouseDown(){
        movePlane = new Plane(Vector3.up, transform.position);
        offset = mouseWorldPosition() - transform.position; 
        
    }

    Vector3 mouseWorldPosition(){
        //cast a ray along the camera to the plane
        Ray rayToPlane = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDis;
        movePlane.Raycast(rayToPlane, out rayDis);
        return rayToPlane.GetPoint(rayDis);
    }
    // when the mouse is clicked on a collider and still holding it
    void OnMouseDrag(){
        transform.position = mouseWorldPosition() - offset;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
