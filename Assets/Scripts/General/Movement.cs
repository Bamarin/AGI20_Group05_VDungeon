using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GridPointAttching grids;
    [Range(10f, 50f)]
    public float rotateSpeed = 40f;
    
    private Material objectMat;

    //keep the offset from the the object center to the mouse clicked position
    private Vector3 offset;
    //the plane where the object is moving on
    private Plane movePlane;

    void checkNearstGridPoint(){
        Vector2Int temp = getGirdIndex();
        int i = temp.x;
        int j = temp.y;
        for (int m = 0; m < grids.gridNumber.y; m++){
            for (int n = 0; n < grids.gridNumber.x; n++){
                grids.gridVertices[m*grids.gridNumber.y+n].GetComponent<MeshRenderer>().material = grids.oriMat;
            }
        }
        if (i >= 0 && i < grids.gridNumber.x && j >=  0 && j < grids.gridNumber.y){
            grids.gridVertices[j*grids.gridNumber.y+i].GetComponent<MeshRenderer>().material = grids.nearMat;
        }
    }

    // get the mouse position in world coordinate
    Vector3 mouseWorldPosition(){
        //cast a ray along the camera to the plane
        Ray rayToPlane = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDis;
        movePlane.Raycast(rayToPlane, out rayDis);
        return rayToPlane.GetPoint(rayDis);
    }
    // get the index of the nearest grid point
    Vector2Int getGirdIndex(){
        int i = Mathf.FloorToInt(((transform.position.x - (grids.leftBottomPosition.x - grids.gridSize/2f)) / grids.gridSize));
        int j = Mathf.FloorToInt(((transform.position.z - (grids.leftBottomPosition.y - grids.gridSize/2f)) / grids.gridSize));
        return new Vector2Int(i, j);
    }

    // when hovering the object, highlight
    void OnMouseOver(){
        GetComponent<MeshRenderer>().material = grids.nearMat;
    }

    // when the mouse is no longer hovering the object, cancel the highlight
    void OnMouseExit(){
        GetComponent<MeshRenderer>().material = objectMat;
    }

    // when the mouse is clicked on a collider
    void OnMouseDown(){
        movePlane = new Plane(Vector3.up, transform.position);
        offset = mouseWorldPosition() - transform.position; 
    }

    // when the mouse is clicked on a collider and still holding it,
    // move the object and show the nearest grid point
    void OnMouseDrag(){
        transform.position = mouseWorldPosition() - offset;
        checkNearstGridPoint();
    }

    // when the mouse exit the collider, attach to the grid vertice
    void OnMouseUpAsButton(){
        Vector2Int temp = getGirdIndex();
        transform.position = new Vector3(grids.leftBottomPosition.x + temp.x*grids.gridSize, 
                                        transform.position.y,
                                        grids.leftBottomPosition.y + temp.y*grids.gridSize);
        //change material of the vertice and the object back to original
        grids.gridVertices[temp.y*grids.gridNumber.y+temp.x].GetComponent<MeshRenderer>().material = grids.oriMat;
    }

    // Start is called before the first frame update
    void Start()
    {
        objectMat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    { 
        Vector3 aroundAxis = new Vector3(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f);
        transform.Rotate(aroundAxis * rotateSpeed * Time.deltaTime);
    }
}
