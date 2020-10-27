using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Draggable : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    [Tooltip("Allow this Draggable to be edited during play mode.")]
    public bool enableEdit = false;
    [Tooltip("Whether this Draggable should require an available path for move operations.")]
    public bool requiresPath = false;
    [Tooltip("Whether this draggable should keep its original rotation during move operations.")]
    public bool fixedRotation = true;

    // *** INTERNAL VARIABLES ***

    public Entity AttachedEntity { get; private set; }

    private List<Renderer> entityRenderers;
    private Entity gridHighlight;

    private bool mouseHover = false;
    private bool mouseLocked = false;

    //keep the offset from the the object center to the mouse clicked position
    private Vector3 offset;
    //the plane where the object is moving on
    private Plane movePlane;
    // for the arrow pointing from source to destination
    private LineRenderer arrow;
    private Vector3 arrowOrigin;
    private Vector3 arrowTarget;
    private float arrowHeadPer = 0.2f;

    // *** INTERNAL FUNCTIONS ***

    private void CreateGridHighlight()
    {
        // TODO: All this highlight code should be moved to an specific class once it becomes needed elsewhere.
        if (gridHighlight == null)
        {
            GameObject newObject = (GameObject)Instantiate(Resources.Load("Prefabs/UI/GridHighlight"));
            newObject.transform.position = transform.position;

            gridHighlight = newObject.GetComponent<Entity>();
            gridHighlight.Initialize(AttachedEntity.ParentGrid);
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
        if (enableEdit && (mouseHover || mouseLocked))
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


    // *** EVENTS ***

    void OnMouseOver()
    {
        if (enableEdit)
        {
            mouseHover = true;
            UpdateRenderers();
        }
    }

    void OnMouseExit()
    {
        mouseHover = false;
        UpdateRenderers();
    }

    // When the mouse is clicked on a collider
    void OnMouseDown()
    {
        if (enableEdit)
        {
            AttachedEntity.SetBookmark();

            mouseLocked = true;
            UpdateRenderers();

            movePlane = new Plane(Vector3.up, transform.position);
            offset = MouseWorldPosition() - transform.position;
            CreateGridHighlight();

            arrowOrigin = transform.position;
            arrowOrigin.y = 0.2f; //make the arrow plane a little bit higher than the game board
        }
    }

    // when the mouse is clicked on a collider and still holding it,
    // move the object and show the nearest grid point
    void OnMouseDrag()
    {
        if (mouseLocked)
        {
            AttachedEntity.Move(AttachedEntity.ParentGrid.WorldToGrid(MouseWorldPosition()));

            UpdateRenderers();
            UpdateGridHighlight();

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

    // when the mouse exit the collider, attach to the grid vertice
    void OnMouseUp()
    {
        if (mouseLocked)
        {
            Vector2Int targetCoordinates = gridHighlight.coordinates;
            DestroyGridHighlight();

            bool isReachable = false;
            if (requiresPath)
            {
                // Check if path between old and new position is possible
                GridPathfinder ptf = new GridPathfinder(AttachedEntity.ParentGrid);
                List<Vector2Int> path = ptf.GetPath(AttachedEntity.coordinates, targetCoordinates);
                isReachable = path != null;
            }
            else
            {
                // Check if the new position is vacant
                isReachable = !AttachedEntity.ParentGrid.CheckCollisionFlags(targetCoordinates, AttachedEntity.GetCollisionFlags());
            }

            if (isReachable)
            {
                // Can move to new position
                AttachedEntity.Move(targetCoordinates);
                // Update collision data
                AttachedEntity.UpdateCollisionChange();
            }
            else
            {
                // Cannot move to new position - go back to old position
                AttachedEntity.LoadBookmark();
            }

            mouseLocked = false;
            UpdateRenderers();

            arrow.positionCount = 0;
        }
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        AttachedEntity = GetComponent<Entity>();
        if (AttachedEntity == null)
        {
            Debug.LogWarning("Draggable object " + name + " has no Entity (or Entity-based) component attached.");
        }

        entityRenderers = new List<Renderer>();
        entityRenderers.AddRange(GetComponentsInChildren<Renderer>());

        UpdateRenderers();

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

    }
}