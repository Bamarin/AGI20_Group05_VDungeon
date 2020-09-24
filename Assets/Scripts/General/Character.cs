using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    // *** STATIC REFERENCES ***

    private static Material MAT_ACTIVE;
    private static Material MAT_SELECTED;

    // Load static references to materials *once*
    private static void LoadMaterials()
    {
        if (MAT_ACTIVE == null) MAT_ACTIVE = Resources.Load<Material>("Materials/UI/CharActive");
        if (MAT_SELECTED == null) MAT_SELECTED = Resources.Load<Material>("Materials/UI/CharSelected");
    }


    // *** PROPERTY FIELDS ***

    public float rotateSpeed = 40f;


    // *** INTERNAL VARIABLES ***

    private new Renderer renderer;
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
            highlight.Initialize(ParentGrid);
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

    // *** UTILITY FUNCTIONS ***

    // Get the mouse position in world coordinates
    Vector3 MouseWorldPosition(){
        //cast a ray along the camera to the plane
        Ray rayToPlane = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDis;
        movePlane.Raycast(rayToPlane, out rayDis);
        return rayToPlane.GetPoint(rayDis);
    }

    private void UpdateMaterial()
    {
        // If the object is being hovered or dragged, highlight
        if (mouseHover || mouseLocked)
        {
            renderer.material = MAT_SELECTED;
        }
        // Otherwise, remove the highlight
        else
        {
            renderer.material = MAT_ACTIVE;
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

    // When the mouse is clicked on a collider
    void OnMouseDown(){
        mouseLocked = true;
        UpdateMaterial();

        movePlane = new Plane(Vector3.up, transform.position);
        offset = MouseWorldPosition() - transform.position;
        CreateGridHighlight();
    }

    // when the mouse is clicked on a collider and still holding it,
    // move the object and show the nearest grid point
    void OnMouseDrag(){
        transform.position = MouseWorldPosition() - offset;
        UpdateHighlight();
    }

    // when the mouse exit the collider, attach to the grid vertice
    void OnMouseUp()
    {
        if (mouseLocked)
        {
            RemoveGridHighlight();
            MoveToNearest();

            mouseLocked = false;
            UpdateMaterial();
        }
    }

    private void Awake()
    {
        LoadMaterials();   
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    { 
        Vector3 aroundAxis = new Vector3(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f);
        transform.Rotate(aroundAxis * rotateSpeed * Time.deltaTime);
    }
}
