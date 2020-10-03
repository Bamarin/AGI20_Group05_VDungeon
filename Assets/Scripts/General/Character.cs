using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    // *** PROPERTY FIELDS ***

    public float mouseSensitivity = 3f;
    public GameObject playerHead;


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
    // for the arrow pointing from source to destination
    private LineRenderer arrow;
    private Vector3 arrowOrigin;
    private Vector3 arrowTarget;
    private float arrowHeadPer = 0.2f;

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
    Vector3 MouseWorldPosition()
    {
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

    // Enable control the first person view through mouse
    private void FPScontrol()
    {
        // change per frame
        float rotationX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotationY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //currently disable the head rotation up and down
        //Vector3 headRotation = playerHead.transform.rotation.eulerAngles;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        // up-and-down for character's head
        //headRotation.x -= rotationY;
        //headRotation.z = 0;

        // left-and-right for character
        playerRotation.y += rotationX;

        //playerHead.rotation = Quaternion.Euler(headRotation);
        transform.rotation = Quaternion.Euler(playerRotation);
    }


    // *** EVENTS ***

    void OnMouseOver()
    {
        if (interactable)
        {
            mouseHover = true;
            UpdateMaterials();
        }
    }

    void OnMouseExit()
    {
        mouseHover = false;
        UpdateMaterials();
    }

    // When the mouse is clicked on a collider
    void OnMouseDown()
    {
        if (interactable)
        {
            mouseLocked = true;
            UpdateMaterials();

            movePlane = new Plane(Vector3.up, transform.position);
            offset = MouseWorldPosition() - transform.position;
            CreateGridHighlight();

            arrowOrigin = transform.position;
            arrowOrigin.y = 0.2f;
        }
    }

    // when the mouse is clicked on a collider and still holding it,
    // move the object and show the nearest grid point
    void OnMouseDrag()
    {
        if (mouseLocked)
        {
            transform.position = MouseWorldPosition() - offset;
            UpdateHighlight();

            arrowTarget = Grid.GridToLocal(Grid.LocalToGrid(transform.position), 0.2f);
            arrow.positionCount = 4;
            arrow.SetPositions(new Vector3[] {
              arrowOrigin
              , Vector3.Lerp(arrowOrigin, arrowTarget, 0.999f - arrowHeadPer)
              , Vector3.Lerp(arrowOrigin, arrowTarget, 1 - arrowHeadPer)
              , arrowTarget });
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

            arrow.positionCount = 0;
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
        if (interactable)
        {
            if (!SystemInfo.supportsGyroscope)
            {
                //enable mouse to control the first person view when the gyroscope is not avaliable
                if (Input.GetMouseButton(1))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    // enable fps when the right click is holding
                    FPScontrol();
                }
                else{
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
}
