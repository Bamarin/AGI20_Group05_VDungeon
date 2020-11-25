using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Interactable : MonoBehaviour
{
    private static int ANIM_TRIGGER_ID = Animator.StringToHash("Activate");

    [Tooltip("If enabled, this object will only accept an interaction once.")]
    public bool oneTimeInteraction = false;

    [Tooltip("If enabled, interactions will toggle this object's collision status.")]
    public bool toggleCollision = false;

    [Tooltip("Objects that should be animated when interacted with.")]
    public Animator[] objectsToAnimate;

    [Tooltip("Objects that should be enabled/disabled when interacted with.")]
    public GameObject[] objectsToToggle;

    

    public Entity AttachedEntity { get; private set; }
    public Prop AttachedProp { get; private set; }
    public Wall AttachedWall { get; private set; }


    private List<Renderer> entityRenderers;

    private bool isBusy = false;
    private bool hasBeenInteractedWith = false;
    private bool isHovered = false;


    // *** INTERNAL FUNCTIONS ***

    private bool IsCurrentlyInteractable()
    {
        // UI blocks editing
        if (EventSystem.current.IsPointerOverGameObject())
            return false;

        // No interaction allowed while edit mode is active
        if (WorldEditor.WorldEditorManager.IsWorldEditorActive)
            return false;

        // Only one interaction allowed
        if (oneTimeInteraction && hasBeenInteractedWith)
            return false;

        return !isBusy;
    }

    private void UpdateRenderers()
    {
        // If the object is being hovered or dragged, highlight
        if (isHovered)
        {
            UpdateMaterial(Color.cyan);
        }
        // Otherwise, remove the highlight
        else
        {
            UpdateMaterial(Color.white);
        }
    }

    private void UpdateMaterial(Color color)
    {
        foreach (var item in entityRenderers)
        {
            item.material.color = color;
        }
    }

    private void RunInteractions()
    {
        if (toggleCollision)
            AttachedEntity.ToggleCollision();

        foreach (var animator in objectsToAnimate)
        {
            animator.SetTrigger(ANIM_TRIGGER_ID);
        }

        foreach (var item in objectsToToggle)
        {
            item.SetActive(!item.activeSelf);
        }
    }

    // *** EVENTS ***

    void OnMouseOver()
    {
        if (IsCurrentlyInteractable())
        {
            isHovered = true;
            UpdateRenderers();
        }
    }

    void OnMouseExit()
    {
        isHovered = false;
        UpdateRenderers();
    }

    // When the mouse is clicked on a collider
    void OnMouseDown()
    {
        if (IsCurrentlyInteractable())
        {
            RunInteractions();
        }
    }



    // Use this for initialization
    void Start()
    {
        AttachedEntity = GetComponent<Entity>();
        AttachedProp = GetComponent<Prop>();
        AttachedWall = GetComponent<Wall>();

        entityRenderers = new List<Renderer>();
        entityRenderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the mouse hovered over UI
        if (isHovered)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                OnMouseExit();
        }
    }
}
