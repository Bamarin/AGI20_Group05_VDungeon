using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A special type of Prop that is positioned between cells on a Grid
public class Wall : Prop
{
    // *** PROPERTY FIELDS ***

    public bool isCornerPiece = false;


    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public override void Move(Vector2Int newCoordinates)
    {
        coordinates = newCoordinates;
        UpdatePosition();
    }

    // Updates the wall's local position to match its current grid coordinates.
    public override void UpdatePosition()
    {
        transform.localPosition = Grid.GridToLocal(coordinates, verticalPosition) + OrientationOffset();
    }

    // Moves the wall to the nearest grid coordinates based on its current local position.
    public override void MoveToNearest()
    {
        Move(Grid.LocalToGrid(transform.localPosition - OrientationOffset()));
    }

    // Updates the wall's rotation and position to match its current grid orientation.
    public override void UpdateOrientation()
    {
        transform.localEulerAngles = new Vector3(0, OrientationAngle());
        UpdatePosition();
    }

    // Get the collision flags for this wall piece based on its type and orientation.
    protected override Grid.CollisionFlags GetPersistentCollisionFlags()
    {
        // NOTE: Corner pieces are set up in a clockwise fashion
        switch (orientation)
        {
            case Grid.Orientation.North:
                return isCornerPiece ? (Grid.CollisionFlags.North | Grid.CollisionFlags.East) : Grid.CollisionFlags.North;
            case Grid.Orientation.East:
                return isCornerPiece ? (Grid.CollisionFlags.East | Grid.CollisionFlags.South) : Grid.CollisionFlags.East;
            case Grid.Orientation.South:
                return isCornerPiece ? (Grid.CollisionFlags.South | Grid.CollisionFlags.West) : Grid.CollisionFlags.South;
            case Grid.Orientation.West:
                return isCornerPiece ? (Grid.CollisionFlags.West | Grid.CollisionFlags.North) : Grid.CollisionFlags.West;
        }
        return Grid.CollisionFlags.None;
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
