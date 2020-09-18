using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWall : GridObject
{
    // *** PROPERTY FIELDS ***

    public bool isCornerPiece = false;

    // *** UTILITY FUNCTIONS ***

    // Updates this object's local position to match its grid wall position and orientation.
    public override void SnapToGrid()
    {
        transform.localPosition = WorldGridManager.GridToLocal(gridPosition) + GetOrientationOffset();
    }

    // Updates this object's grid position to match its local position. (moves object to nearest cell)
    public override void SnapToLocal()
    {
        MoveToGridPosition(WorldGridManager.LocalToGrid(transform.localPosition - GetOrientationOffset()));
    }

    // Updates this object's local position and Y-axis rotation to match its grid orientation.
    public override void SnapToOrientation()
    {
        transform.localEulerAngles = new Vector3(0, GetOrientationAngle());
        SnapToGrid();
    }

    protected Vector3 GetOrientationOffset()
    {
        float halfCellSize = WorldGridManager.CELL_SIZE / 2;
        float cornerOffset = isCornerPiece ? halfCellSize : 0;

        switch (gridOrientation)
        {
            case Orientation.North:
                return new Vector3(cornerOffset, 0, halfCellSize);
            case Orientation.East:
                return new Vector3(halfCellSize, 0, -cornerOffset);
            case Orientation.South:
                return new Vector3(-cornerOffset, 0, -halfCellSize);
            case Orientation.West:
                return new Vector3(-halfCellSize, 0, cornerOffset);
            // "Random" orientation should not be used for walls
        }

        Debug.LogWarning("Invalid orientation in GridWall object: " + gameObject.name + ". Random orientation should not be used with grid walls.");
        return Vector3.zero;
    }

    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
