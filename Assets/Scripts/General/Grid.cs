using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public enum Orientation
    {
        North,
        East,
        South,
        West,
        Random,
    }

    [Flags]
    public enum CollisionFlags : byte
    {
        None            = 0,
        Center          = 0b00001,
        North           = 0b00010,
        South           = 0b00100,
        East            = 0b01000,
        West            = 0b10000,
        AllDirections   = 0b11110,
        All             = 0b11111,
    }

    // *** PROPERTY FIELDS ***

    // Reasoning for this value: considering 1 unit = 1 meter and the default DnD cell size of 5 feet =~ 1.5 meters
    // However this could be made configurable in the future
    public const float CELL_SIZE = 1.5f;

    // Grid size defines the *radius* within which collisions are calculated
    // Entities can occupy spaces outside of this range, however collisions will not be calculated for them
    public Vector2Int gridSize = new Vector2Int(5, 5);


    // *** UTILITY VARIABLES ***

    // Stores all objects attached to this grid
    private List<Entity> entityList;

    // Stores grid cell collision data
    public CollisionFlags[,] gridCollisions;


    // *** UTILITY FUNCTIONS ***

    // Convert grid coordinates to local position. (Y axis is set to verticalPosition)
    public static Vector3 GridToLocal(Vector2Int gridPosition, float verticalPosition = 0)
    {
        return new Vector3(gridPosition.x * CELL_SIZE, verticalPosition, gridPosition.y * CELL_SIZE);
    }

    // Convert local position to the nearest grid coordinates. (Y axis is ignored)
    public static Vector2Int LocalToGrid(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt((position.x + (CELL_SIZE / 2)) / CELL_SIZE), Mathf.FloorToInt((position.z + (CELL_SIZE / 2)) / CELL_SIZE));
    }

    // Convert grid coordinates to world position. (Y axis is set to verticalPosition)
    public Vector3 GridToWorld(Vector2Int gridPosition, float verticalPosition = 0)
    {
        return transform.TransformPoint(GridToLocal(gridPosition, verticalPosition));
    }

    // Convert world position to the nearest grid coordinates. (Y axis is ignored)
    public Vector2Int WorldToGrid(Vector3 position)
    {
        return LocalToGrid(transform.InverseTransformPoint(position));
    }

    // Convert grid orientation to local angle. (around Y-axis)
    public static float OrientationToAngle(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.North:
                return 0;
            case Orientation.East:
                return 90;
            case Orientation.South:
                return 180;
            case Orientation.West:
                return 270;
        }
        return 0;
    }

    // Set collision flags in a grid cell. No effect if the coordinates are out of bounds of the collider array.
    public void SetCollisionFlags(Vector2Int gridPosition, CollisionFlags flags)
    {
        Vector2Int index = GridToCollisionArray(gridPosition);
        if (IsInCollisionArray(index))
        {
            gridCollisions[index.x, index.y] = flags;
        }
    }
    
    // Add specific collision flags to a grid cell. Existing flags are not changed.
    public void AddCollisionFlags(Vector2Int gridPosition, CollisionFlags flags)
    {
        Vector2Int index = GridToCollisionArray(gridPosition);
        if (IsInCollisionArray(index))
        {
            gridCollisions[index.x, index.y] |= flags;
        }
    }

    // Remove specific collision flags from a grid cell.
    public void RemoveCollisionFlags(Vector2Int gridPosition, CollisionFlags flags)
    {
        Vector2Int index = GridToCollisionArray(gridPosition);
        if (IsInCollisionArray(index))
        {
            gridCollisions[index.x, index.y] &= ~flags;
        }
    }

    // Get all collision flags from a grid cell. Out of bounds positions will always return central collision.
    public CollisionFlags GetCollisionFlags(Vector2Int gridPosition)
    {
        Vector2Int index = GridToCollisionArray(gridPosition);
        if (IsInCollisionArray(index))
        {
            return gridCollisions[index.x, index.y];
        }

        // Out of bounds always returns full collision; prevents entities with collision from being moved out of bounds.
        return CollisionFlags.All;
    }

    // Returns true if *at least one* of the specified collision flags are present in a grid cell.
    // For walls, it also checks neighboring cells for opposing walls.
    public bool CheckCollisionFlags(Vector2Int gridPosition, CollisionFlags flagsToCheck)
    {
        if (CheckCollisionFlags(flagsToCheck, CollisionFlags.AllDirections))
            return CheckWallCollisionFlags(gridPosition, flagsToCheck);

        return CheckCollisionFlags(GetCollisionFlags(gridPosition), flagsToCheck);
    }

    public static bool CheckCollisionFlags(CollisionFlags value, CollisionFlags flagsToCheck)
    {
        return (value & flagsToCheck) != 0;
    }

    private bool CheckWallCollisionFlags(Vector2Int gridPosition, CollisionFlags flagsToCheck)
    {
        // Check central cell for any conflicting collisions
        if (CheckCollisionFlags(GetCollisionFlags(gridPosition), flagsToCheck))
            return true;

        // Check the neighboring cells for opposing collisions that apply
        if (CheckCollisionFlags(flagsToCheck, CollisionFlags.North))
        {
            if (CheckCollisionFlags(GetCollisionFlags(gridPosition + Vector2Int.up), CollisionFlags.South))
                return true;
        }
        if (CheckCollisionFlags(flagsToCheck, CollisionFlags.South))
        {
            if (CheckCollisionFlags(GetCollisionFlags(gridPosition + Vector2Int.down), CollisionFlags.North))
                return true;
        }
        if (CheckCollisionFlags(flagsToCheck, CollisionFlags.East))
        {
            if (CheckCollisionFlags(GetCollisionFlags(gridPosition + Vector2Int.right), CollisionFlags.West))
                return true;
        }
        if (CheckCollisionFlags(flagsToCheck, CollisionFlags.West))
        {
            if (CheckCollisionFlags(GetCollisionFlags(gridPosition + Vector2Int.left), CollisionFlags.East))
                return true;
        }

        // No conflicting collisions found.
        return false;
    }

    // Convert grid coordinates to collision array index values. (internal use only)
    private Vector2Int GridToCollisionArray(Vector2Int gridPosition)
    {
        return new Vector2Int(gridPosition.x + gridSize.x, gridPosition.y + gridSize.y);
    }

    private bool IsInCollisionArray(Vector2Int index)
    {
        return (index.x >= 0 && index.y >= 0 && index.x < gridCollisions.GetLength(0) && index.y < gridCollisions.GetLength(1));
    }

    // *** INITIALIZATION FUNCTIONS ***
    private void InitializeEntities()
    {
        entityList = new List<Entity>();
        gridCollisions = new CollisionFlags[(gridSize.x*2)+1, (gridSize.y*2)+1];

        // Locate all existing entities on this grid
        entityList.AddRange(GetComponentsInChildren<Entity>());
        Debug.Log(entityList.Count + " entities found in grid: " + name);

        // Initialize and update all entities on the grid
        foreach (var item in entityList)
        {
            item.Initialize(this);
            item.UpdateEntity();

            CollisionFlags flags = item.GetCollisionFlags();
            if (flags != CollisionFlags.None)
            {
                // Update grid cell collision data
                AddCollisionFlags(item.coordinates, flags);
            }
        }
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        InitializeEntities();
    }

    // Update is called once per frame
    void Update()
    {
    }


#if UNITY_EDITOR
    // *** DEBUG FUNCTIONS ***

    public bool showGrid = false;
    public bool showCollisions = false;
    public bool showWallCollisions = false;

    private void OnDrawGizmos()
    {
        if (showGrid) DEBUG_DrawGrid();
        if (showCollisions) DEBUG_DrawCollisions();
        if (showWallCollisions) DEBUG_DrawWallCollisions();
    }

    private void DEBUG_DrawGrid()
    {
        Gizmos.color = Color.white;
        for (int x = -gridSize.x; x < gridSize.x + 1; x++)
        {
            for (int y = -gridSize.y; y < gridSize.y + 1; y++)
            {
                Gizmos.DrawWireCube(GridToWorld(new Vector2Int(x,y), CELL_SIZE / 2), new Vector3(CELL_SIZE, CELL_SIZE, CELL_SIZE));
            }
        }
    }

    private void DEBUG_DrawCollisions()
    {
        if (gridCollisions == null) return;

        Gizmos.color = Color.red;

        for (int x = 0; x < gridCollisions.GetLength(0); x++)
        {
            for (int y = 0; y < gridCollisions.GetLength(1); y++)
            {
                if (CheckCollisionFlags(gridCollisions[x,y], CollisionFlags.Center))
                {
                    Vector2Int coordinates = new Vector2Int(x - gridSize.x, y - gridSize.y);
                    Gizmos.DrawWireCube(GridToWorld(coordinates, CELL_SIZE / 2), new Vector3(CELL_SIZE, CELL_SIZE, CELL_SIZE));
                }
            }
        }
    }

    private void DEBUG_DrawWallCollisions()
    {
        if (gridCollisions == null) return;

        Gizmos.color = Color.red;

        for (int x = 0; x < gridCollisions.GetLength(0); x++)
        {
            for (int y = 0; y < gridCollisions.GetLength(1); y++)
            {
                Vector3 center = GridToWorld(new Vector2Int(x - gridSize.x, y - gridSize.y), CELL_SIZE / 2);
                if (CheckCollisionFlags(gridCollisions[x, y], CollisionFlags.North))
                {
                    Gizmos.DrawWireCube(new Vector3(center.x, center.y, center.z + (CELL_SIZE / 2)), new Vector3(CELL_SIZE, CELL_SIZE, 0.1f));
                }
                if (CheckCollisionFlags(gridCollisions[x, y], CollisionFlags.East))
                {
                    Gizmos.DrawWireCube(new Vector3(center.x + (CELL_SIZE / 2), center.y, center.z), new Vector3(0.1f, CELL_SIZE, CELL_SIZE));
                }
                if (CheckCollisionFlags(gridCollisions[x, y], CollisionFlags.South))
                {
                    Gizmos.DrawWireCube(new Vector3(center.x, center.y, center.z - (CELL_SIZE / 2)), new Vector3(CELL_SIZE, CELL_SIZE, 0.1f));
                }
                if (CheckCollisionFlags(gridCollisions[x, y], CollisionFlags.West))
                {
                    Gizmos.DrawWireCube(new Vector3(center.x - (CELL_SIZE / 2), center.y, center.z), new Vector3(0.1f, CELL_SIZE, CELL_SIZE));
                }
            }
        }
    }
#endif
}
