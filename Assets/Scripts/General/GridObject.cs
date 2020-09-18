using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public enum Orientation
    {
        North,
        East,
        South,
        West,
        Random,
    }

    // *** PROPERTY FIELDS ***

    public Vector2Int gridPosition;
    public Orientation gridOrientation;


    // *** INTERNAL VARIABLES ***

    private int randomRotationValue;


    // *** UTILITY FUNCTIONS ***

    // Moves this object to a new grid position. (also updates its local position)
    public void MoveToGridPosition(Vector2Int position)
    {
        gridPosition = position;
        SnapToGrid();
    }

    public void MoveToGridOrientation(Orientation orientation)
    {
        gridOrientation = orientation;
        SnapToOrientation();
    }

    // Updates this object's local position to match its grid position.
    public virtual void SnapToGrid()
    {
        transform.localPosition = WorldGridManager.GridToLocal(gridPosition);
    }

    // Updates this object's grid position to match its local position. (moves object to nearest cell)
    public virtual void SnapToLocal()
    {
        MoveToGridPosition(WorldGridManager.LocalToGrid(transform.localPosition));
    }

    // Updates this object's Y-axis rotation to match its grid orientation.
    public virtual void SnapToOrientation()
    {
        transform.localEulerAngles = new Vector3(0, GetOrientationAngle());
    }

    // Equivalent to calling SnapToGrid and SnapToOrientation, ensures this object is positioned correctly on the grid.
    public void SnapToAll()
    {
        SnapToGrid();
        SnapToOrientation();
    }

    // Convert grid orientation to an usable angle value.
    protected float GetOrientationAngle()
    {
        switch (gridOrientation)
        {
            case Orientation.North:
                return 0;
            case Orientation.East:
                return 90;
            case Orientation.South:
                return 180;
            case Orientation.West:
                return 270;
            case Orientation.Random:
                return randomRotationValue;
        }
        return 0;
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    private void Awake()
    {
        // Load a consistent value for random rotation for this object
        randomRotationValue = Random.Range(0, 360);
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
