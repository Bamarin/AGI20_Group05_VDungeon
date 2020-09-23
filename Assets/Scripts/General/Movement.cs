using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GridPointAttching grids;
    [Range(10f, 50f)]
    public float rotateSpeed = 40f;
    
    private Material objectMat;
    private new Renderer renderer;

    private Entity entity;
    private Entity highlight;

    private bool mouseHover = false;
    private bool mouseLocked = false;

    //keep the offset from the the object center to the mouse clicked position
    private Vector3 offset;
    //the plane where the object is moving on
    private Plane movePlane;

    private void CreateGridHighlight()
    {
        // TODO: All this highlight code should be moved to an specific class once it becomes needed elsewhere.
        if (highlight == null)
        {
            GameObject newObject = (GameObject)Instantiate(Resources.Load("Prefabs/UI/GridHighlight"));
            newObject.transform.position = transform.position;

            highlight = newObject.GetComponent<Entity>();
            highlight.Initialize(entity.ParentGrid);
            highlight.MoveToNearest();
        }
    }

    private void RemoveGridHighlight()
    {
        Destroy(highlight.gameObject);
        highlight = null;
    }

    private void UpdateHighlight()
    {
        highlight.Move(Grid.LocalToGrid(transform.localPosition));
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

    private void UpdateMaterial()
    {
        // If the object is being hovered or dragged, highlight
        if (mouseHover || mouseLocked)
        {
            renderer.material = grids.nearMat;
        }
        // Otherwise, remove the highlight
        else
        {
            renderer.material = objectMat;
        }
    }

    void OnMouseOver(){
        mouseHover = true;
        UpdateMaterial();
    }

    void OnMouseExit(){
        mouseHover = false;
        UpdateMaterial();
    }

    // when the mouse is clicked on a collider
    void OnMouseDown(){
        mouseLocked = true;
        UpdateMaterial();

        movePlane = new Plane(Vector3.up, transform.position);
        offset = mouseWorldPosition() - transform.position;
        CreateGridHighlight();
    }

    // when the mouse is clicked on a collider and still holding it,
    // move the object and show the nearest grid point
    void OnMouseDrag(){
        transform.position = mouseWorldPosition() - offset;
        UpdateHighlight();
    }

    // when the mouse exit the collider, attach to the grid vertice
    void OnMouseUp()
    {
        if (mouseLocked)
        {
            RemoveGridHighlight();
            entity.MoveToNearest();

            mouseLocked = false;
            UpdateMaterial();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        entity = GetComponent<Entity>();

        objectMat = renderer.material;
    }

    // Update is called once per frame
    void Update()
    { 
        Vector3 aroundAxis = new Vector3(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f);
        transform.Rotate(aroundAxis * rotateSpeed * Time.deltaTime);
    }
}
