using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Draggable : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    [Tooltip("Allow this Draggable to be edited during play mode.")]
    public bool enableEdit = false;
    [Tooltip("Whether this Draggable can only be edited during the dedicated edit mode.")]
    public bool requiresEditMode = true;
    [Tooltip("Whether this Draggable should require an available path for move operations.")]
    public bool requiresPath = false;
    [Tooltip("Whether this draggable should keep its original rotation during move operations.")]
    public bool fixedRotation = true;

    // *** INTERNAL VARIABLES ***

    public Entity AttachedEntity { get; private set; }
    public Prop AttachedProp { get; private set; }

    private List<Renderer> entityRenderers;
    private Entity gridHighlight;

    public static bool HasSelection { get; private set; }

    private bool isHovered = false;
    private bool isSelected = false;
    private bool firstSelectionFrame = false;

    //the plane where the object is moving on
    private Plane movePlane;
    // for the arrow pointing from source to destination
    private LineRenderer arrow;
    private Vector3 arrowOrigin;
    private Vector3 arrowTarget;
    private float arrowHeadPer = 0.2f;

    // *** INTERNAL FUNCTIONS ***

    private bool IsCurrentlyEditable()
    {
        // Nothing is editable while something else is selected
        if (HasSelection)
            return false;

        // Objects that require edit mode are not editable otherwise
        if (requiresEditMode && !WorldEditor.WorldEditorManager.IsWorldEditorActive)
            return false;

        return enableEdit;
    }

    private void CreateGridHighlight()
    {
        // TODO: All this highlight code should be moved to an specific class once it becomes needed elsewhere.
        if (gridHighlight == null)
        {
            GameObject newObject = (GameObject)Instantiate(Resources.Load("Prefabs/UI/GridHighlight"));
            newObject.transform.position = transform.position;

            gridHighlight = newObject.GetComponent<Entity>();
            gridHighlight.Move(AttachedEntity.coordinates);
        }
    }

    private void DestroyGridHighlight()
    {
        Destroy(gridHighlight.gameObject);
        gridHighlight = null;
    }

    private void UpdateGridHighlight()
    {
        gridHighlight.Move(AttachedEntity.coordinates);
    }

    private void UpdateRenderers()
    {
        // If the object is being hovered or dragged, highlight
        if (enableEdit && (isHovered || isSelected))
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
        foreach (var item in entityRenderers)
        {
            item.material.color = color;
        }
    }

    private void Delete()
    {
        if (gridHighlight != null)
        {
            Destroy(gridHighlight.gameObject);
        }

        if (isSelected)
        {
            HasSelection = false;
        }

        Destroy(gameObject);
    }

    // *** UTILITY FUNCTIONS ***

    // Get the mouse position in world coordinates
    Vector3 MouseWorldPosition()
    {
        //cast a ray along the camera to the plane
        Ray rayToPlane = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDis;
        movePlane.Raycast(rayToPlane, out rayDis);
        return rayToPlane.GetPoint(rayDis);
    }

    // Enables or disables editing for this entity, and updates its appearance accordingly.
    public void EnableEditing(bool enable = true)
    {
        enableEdit = enable;
        UpdateRenderers();
    }

    // Use this when creating a new object that must immediately be placed.
    public void InitializeInDragMode()
    {
        // Not possible to just call Select(), since it requires the object to be initialized
        // Select() will be called in Start() instead
        isSelected = true; ;
    }


    // *** EVENTS ***

    void OnMouseOver()
    {
        if (IsCurrentlyEditable())
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
        if (!isSelected && IsCurrentlyEditable())
        {
            Select();
        }
    }

    // when the mouse is clicked on a collider and still holding it,
    // move the object and show the nearest grid point
    void OnMouseDrag()
    {
        if (isSelected)
        {
            Drag();
        }
    }

    // when the mouse exit the collider, attach to the grid vertice
    void OnMouseUp()
    {
        if (isSelected && !WorldEditor.WorldEditorManager.IsWorldEditorActive)
        {
            Deselect();
        }
    }


    // *** INTERNAL FUNCTIONS ***
    private void Select()
    {
        AttachedEntity.SetBookmark();
        AttachedEntity.ClearCollisionAtBookmark();

        HasSelection = true;
        isSelected = true;
        firstSelectionFrame = true;
        UpdateRenderers();

        movePlane = new Plane(Vector3.up, transform.position);
        CreateGridHighlight();

        arrowOrigin = transform.position;
        arrowOrigin.y = 0.2f; //make the arrow plane a little bit higher than the game board
    }

    private void Drag()
    {
        firstSelectionFrame = false;

        AttachedEntity.Move(Grid.grid.WorldToGrid(MouseWorldPosition()));

        UpdateRenderers();
        UpdateGridHighlight();

        if (!WorldEditor.WorldEditorManager.IsWorldEditorActive)
        {
            arrowTarget = Grid.GridToLocal(AttachedEntity.coordinates, 0.2f);
            arrow.positionCount = 4;
            arrow.SetPositions(new Vector3[] {
              arrowOrigin
              , Vector3.Lerp(arrowOrigin, arrowTarget, 0.999f - arrowHeadPer)
              , Vector3.Lerp(arrowOrigin, arrowTarget, 1 - arrowHeadPer)
              , arrowTarget });

            if (!fixedRotation)
            {
                // Rotate to face movement direction
                Quaternion orientation = Quaternion.LookRotation(arrowTarget - arrowOrigin, Vector3.up);
                transform.rotation = orientation;
            }
        }
    }

    private void Deselect()
    {
        Vector2Int targetCoordinates = gridHighlight.coordinates;

        bool isReachable = false;
        if (requiresPath && !WorldEditor.WorldEditorManager.IsWorldEditorActive)
        {
            // Check if path between old and new position is possible
            GridPathfinder ptf = new GridPathfinder(Grid.grid);
            List<Vector2Int> path = ptf.GetPath(AttachedEntity.BookmarkedCoordinates, targetCoordinates);
            isReachable = path != null;
        }
        else
        {
            // Check if the new position is vacant
            isReachable = !Grid.grid.CheckCollisionFlags(targetCoordinates, AttachedEntity.GetCollisionFlags());
        }

        if (isReachable)
        {
            // Can move to new position
            AttachedEntity.Move(targetCoordinates);
        }
        else
        {
            if (WorldEditor.WorldEditorManager.IsWorldEditorActive)
            {
                // Cannot move to selected position - abort placement
                return;
            }
            else
            {
                // Cannot move to new position - go back to old position
                AttachedEntity.LoadBookmark();
            }
        }

        DestroyGridHighlight();

        // Update collision data
        AttachedEntity.UpdateCollision();

        HasSelection = false;
        isSelected = false;
        UpdateRenderers();

        arrow.positionCount = 0;
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        // AttachedEntity and AttachedProp refer to the same component, only at different levels (Prop allows for orientation control)
        // AttachedEntity is required, but AttachedProp is not
        AttachedEntity = GetComponent<Entity>();
        AttachedProp = GetComponent<Prop>();

        if (AttachedEntity == null)
        {
            Debug.LogWarning("Draggable object " + name + " has no Entity (or Entity-based) component attached.");
        }

        entityRenderers = new List<Renderer>();
        entityRenderers.AddRange(GetComponentsInChildren<Renderer>());

        // If this object was initialized in drag mode, start it right away
        if (isSelected)
        {
            Select();
        }
        else
        {
            UpdateRenderers();
        }

        // initial set for arrow
        arrow = gameObject.AddComponent<LineRenderer>() as LineRenderer;
        arrow.material = new Material(Shader.Find("Sprites/Default"));
        arrow.positionCount = 0;
        // set shape for arrow
        arrow.widthCurve = new AnimationCurve(
             new Keyframe(0, 0.15f)
             , new Keyframe(0.999f - arrowHeadPer, 0.15f)  // neck of arrow
             , new Keyframe(1 - arrowHeadPer, 0.3f)  // max width of arrow head
             , new Keyframe(1, 0f));
        //arrow color
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(0.6f, 1.0f) }
        );
        arrow.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            // Rotation operations are only available for Props
            if (AttachedProp != null)
            {
                // Rotate with 'R'
                if (Input.GetKeyDown(KeyCode.R))
                {
                    AttachedProp.Rotate(Grid.RotateCW(AttachedProp.orientation));
                }
            }

            // Delete with 'Del'
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        if (WorldEditor.WorldEditorManager.IsWorldEditorActive && isSelected)
        {
            if (!firstSelectionFrame && Input.GetMouseButtonDown(0))
            {
                Deselect();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Delete();
            }
            else
            {
                Drag();
            }
        }
    }
}