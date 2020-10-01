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

    // The default material to render this character with when it is not highlighted
    public Material defaultMaterial;
    public float rotateSpeed = 40f;
    // Whether this character can be interacted with or not. Use EnableInteraction() to ensure the character's appearance is updated!
    public bool interactable = false; 

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
        // If the object is inactive, use the default material
        if (!interactable)
        {
            renderer.material = defaultMaterial;
        }
        // If the object is being hovered or dragged, highlight
        else if (mouseHover || mouseLocked)
        {
            renderer.material = MAT_SELECTED;
        }
        // Otherwise, remove the highlight
        else
        {
            renderer.material = MAT_ACTIVE;
        }
    }

    // Enables or disables interaction with this character, and updates its appearance accordingly.
    public void EnableInteraction(bool enable = true)
    {
        interactable = enable;
        UpdateMaterial();
    }


    // *** EVENTS ***

    void OnMouseOver(){
        if (interactable)
        {
            mouseHover = true;
            UpdateMaterial();
        }
    }

    void OnMouseExit(){
        mouseHover = false;
        UpdateMaterial();
    }

    // When the mouse is clicked on a collider
    void OnMouseDown(){
        if (interactable)
        {
            mouseLocked = true;
            UpdateMaterial();

            movePlane = new Plane(Vector3.up, transform.position);
            offset = MouseWorldPosition() - transform.position;
            CreateGridHighlight();
        }
    }

    // when the mouse is clicked on a collider and still holding it,
    // move the object and show the nearest grid point
    void OnMouseDrag(){
        if (mouseLocked)
        {
            transform.position = MouseWorldPosition() - offset;
            UpdateHighlight();
        }
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


    // *** MONOBEHAVIOUR FUNCTIONS ***

    private void Awake()
    {
        LoadMaterials();   
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();

        if (defaultMaterial == null)
        {
            Debug.Log("No material found for " + name + ", loading default from renderer");
            defaultMaterial = renderer.material;
        }

        UpdateMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable)
        {
            Vector3 aroundAxis = new Vector3(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f);
            transform.Rotate(aroundAxis * rotateSpeed * Time.deltaTime);
        }
    }
}
