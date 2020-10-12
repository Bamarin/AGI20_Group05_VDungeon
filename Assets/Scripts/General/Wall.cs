using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A special type of Prop that is positioned between cells on a Grid
public class Wall : Prop
{
    // *** PROPERTY FIELDS ***

    public bool isCornerPiece = false;

    // *** UTILITY FUNCTIONS ***

    // Updates the wall's local position to match its current grid coordinates.
    public override void UpdatePosition()
    {
        transform.localPosition = Grid.GridToLocal(coordinates, verticalPosition) + OrientationOffset();
    }

    // Moves the wall to the nearest grid coordinates based on its current local position.
    public override bool MoveToNearest()
    {
        return Move(Grid.LocalToGrid(transform.localPosition - OrientationOffset()));
    }

    // Updates the wall's rotation and position to match its current grid orientation.
    public override void UpdateOrientation()
    {
        transform.localEulerAngles = new Vector3(0, OrientationAngle());
        UpdatePosition();
    }

    protected Vector3 OrientationOffset()
    {
        float halfCellSize = Grid.CELL_SIZE / 2;
        float cornerOffset = isCornerPiece ? halfCellSize : 0;

        switch (orientation)
        {
            case Grid.Orientation.North:
                return new Vector3(cornerOffset, 0, halfCellSize);
            case Grid.Orientation.East:
                return new Vector3(halfCellSize, 0, -cornerOffset);
            case Grid.Orientation.South:
                return new Vector3(-cornerOffset, 0, -halfCellSize);
            case Grid.Orientation.West:
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
