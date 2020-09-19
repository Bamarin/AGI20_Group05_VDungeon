using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //keep the offset from the the object center to the mouse clicked position
    public Vector3 offset;
    //the plane where the object is moving on
    public Plane movePlane;
    public GridPointAttching grids;

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
        checkNearstGridPoint();
    }

    void checkNearstGridPoint(){
        int i = Mathf.FloorToInt(((transform.position.x - (grids.leftBottomPosition.x - grids.gridSize/2f)) / grids.gridSize));
        int j = Mathf.FloorToInt(((transform.position.z - (grids.leftBottomPosition.y - grids.gridSize/2f)) / grids.gridSize));
        for (int m = 0; m < grids.gridNumber.y; m++){
            for (int n = 0; n < grids.gridNumber.x; n++){
                grids.gridVertices[m*grids.gridNumber.y+n].GetComponent<MeshRenderer>().material = grids.oriMat;
            }
        }
        if (i >= 0 && i < grids.gridNumber.x && j >=  0 && j < grids.gridNumber.y){
            grids.gridVertices[j*grids.gridNumber.y+i].GetComponent<MeshRenderer>().material = grids.nearMat;
        }
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
