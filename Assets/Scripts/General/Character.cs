﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    // *** PROPERTY FIELDS ***

    public float rotateSpeed = 40f;
    // Whether this character can be interacted with or not. Use EnableInteraction() to ensure the character's appearance is updated!
    public bool interactable = false; 

    // *** INTERNAL VARIABLES ***

    private List<Renderer> characterRenderers;
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

    private void UpdateMaterials()
    {
        // If the object is being hovered or dragged, highlight
        if (interactable && (mouseHover || mouseLocked))
        {
            UpdateMaterial(Color.red);
        }
        // Otherwise, remove the highlight
        else
        {
            UpdateMaterial(Color.white);
        }
    }

    private void UpdateMaterial(Color color)
    {
        foreach (var item in characterRenderers)
        {
            item.material.color = color;
        }
    }

    // Enables or disables interaction with this character, and updates its appearance accordingly.
    public void EnableInteraction(bool enable = true)
    {
        interactable = enable;
        UpdateMaterials();
    }


    // *** EVENTS ***

    void OnMouseOver(){
        if (interactable)
        {
            mouseHover = true;
            UpdateMaterials();
        }
    }

    void OnMouseExit(){
        mouseHover = false;
        UpdateMaterials();
    }

    // When the mouse is clicked on a collider
    void OnMouseDown(){
        if (interactable)
        {
            mouseLocked = true;
            UpdateMaterials();

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
            UpdateMaterials();
        }
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    private void Awake()
    {  

    }

    // Start is called before the first frame update
    void Start()
    {
        characterRenderers = new List<Renderer>();
        characterRenderers.AddRange(GetComponentsInChildren<Renderer>());

        UpdateMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable)
        {
            if (!SystemInfo.supportsGyroscope){ //enable keyboard to rotate when the gyroscope is not valide
                Vector3 aroundAxis = new Vector3(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f);
                transform.Rotate(aroundAxis * rotateSpeed * Time.deltaTime);
            }
        }
    }
}
