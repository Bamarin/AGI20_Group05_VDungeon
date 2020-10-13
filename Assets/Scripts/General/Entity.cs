using UnityEngine;
using System.Collections;

// A GameObject whose position is regulated by a Grid
public class Entity : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    public bool hasCollision;
    public Vector2Int coordinates;
    public float verticalPosition;

    public Grid ParentGrid { get; private set; }

    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public virtual bool Move(Vector2Int newCoordinates, bool ignoreCollision = false)
    {
        if (hasCollision && !ignoreCollision)
        {
            // Abort movement if the new position already is occupied
            if (ParentGrid.CheckCollisionFlags(newCoordinates, Grid.CollisionFlags.Center)) return false;

            // Update collision array
            ParentGrid.RemoveCollisionFlags(coordinates, Grid.CollisionFlags.Center);
            ParentGrid.AddCollisionFlags(newCoordinates, Grid.CollisionFlags.Center);
        }
        coordinates = newCoordinates;
        UpdatePosition();

        return true;
    }

    // Updates the entity's local position to match its current grid coordinates.
    public virtual void UpdatePosition()
    {
        transform.localPosition = Grid.GridToLocal(coordinates, verticalPosition);
    }

    // Moves the entity to the nearest grid coordinates based on its current local position.
    public virtual bool MoveToNearest()
    {
        return Move(Grid.LocalToGrid(transform.localPosition));
    }

    // Ensures the entity is correctly positioned and oriented within the grid.
    public virtual void UpdateEntity()
    {
        UpdatePosition();
    }

    // Get the collision flags for this wall piece based on its type and orientation.
    public virtual Grid.CollisionFlags GetCollisionFlags()
    {
        if (hasCollision)
        {
            return Grid.CollisionFlags.Center;
        }

        return Grid.CollisionFlags.None;
    }

    // *** GENERAL FUNCTIONS ***

    // Initializes this Entity to a Grid. Only call once.
    public void Initialize(Grid parentGrid)
    {
        if (ParentGrid == null)
        {
            ParentGrid = parentGrid;
            transform.SetParent(ParentGrid.transform, true);
        }
        else
        {
            Debug.LogWarning("Entity.Initialize() was called more than once on Entity: " + name);
        }
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
