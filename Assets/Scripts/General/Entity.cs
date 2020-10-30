﻿using UnityEngine;
using System.Collections;

// A GameObject whose position is regulated by a Grid
public class Entity : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    public bool hasCollision;
    public Vector2Int coordinates;
    public float verticalPosition;

    public Grid ParentGrid { get; private set; }

    // *** INTERNAL VARIABLES ***

    public Vector2Int BookmarkedCoordinates { get; protected set; }
    public Grid.CollisionFlags BookmarkedCollision { get; protected set; }

    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public virtual void Move(Vector2Int newCoordinates)
    {
        coordinates = newCoordinates;
        UpdatePosition();
    }

    // Updates the entity's local position to match its current grid coordinates.
    public virtual void UpdatePosition()
    {
        transform.localPosition = Grid.GridToLocal(coordinates, verticalPosition);
    }

    // Moves the entity to the nearest grid coordinates based on its current local position.
    public virtual void MoveToNearest()
    {
        Move(Grid.LocalToGrid(transform.localPosition));
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

    // Bookmarks this Entity's current status for easy fallback at a later point.
    // Also relevant for collision data maintenance.
    public virtual void SetBookmark()
    {
        BookmarkedCoordinates = coordinates;
        BookmarkedCollision = GetCollisionFlags();
    }

    // Returns this Entity to a previously bookmarked status.
    public virtual void LoadBookmark()
    {
        Move(BookmarkedCoordinates);
    }

    // Removes the collision data from the bookmarked position.
    public void ClearCollisionAtBookmark()
    {
        ParentGrid.RemoveCollisionFlags(BookmarkedCoordinates, BookmarkedCollision);
    }

    // Updates collision data according to this Entity's current status.
    public void UpdateCollision()
    {
        ParentGrid.AddCollisionFlags(coordinates, GetCollisionFlags());
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
        SetBookmark();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
