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
    private bool[,] gridCollisions;


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

    // Set collision in a grid cell. No effect if the coordinates are out of bounds of the collider array.
    public void SetCollision(Vector2Int gridPosition, bool enable)
    {
        Vector2Int index = GridToCollisionArray(gridPosition);
        if (index.x >= 0 && index.y >= 0 && index.x < gridCollisions.GetLength(0) && index.y < gridCollisions.GetLength(1))
        {
            gridCollisions[index.x, index.y] = enable;
        }
    }

    public bool GetCollision(Vector2Int gridPosition)
    {
        Vector2Int index = GridToCollisionArray(gridPosition);
        if (index.x < gridCollisions.GetLength(0) && index.y < gridCollisions.GetLength(1))
        {
            return gridCollisions[index.x, index.y];
        }

        // Out of bounds always returns true; prevents entities with collision from being moved out of bounds.
        return true;
    }

    // Disables collision at the old position and enables collision at the new position
    public void MoveCollision(Vector2Int oldPosition, Vector2Int newPosition)
    {
        SetCollision(oldPosition, false);
        SetCollision(newPosition, true);
    }

    // Convert grid coordinates to collision array index values. (internal use only)
    private Vector2Int GridToCollisionArray(Vector2Int gridPosition)
    {
        return new Vector2Int(gridPosition.x + gridSize.x, gridPosition.y + gridSize.y);
    }

    // *** INITIALIZATION FUNCTIONS ***
    private void InitializeEntities()
    {
        entityList = new List<Entity>();
        gridCollisions = new bool[(gridSize.x*2)+1, (gridSize.y*2)+1];

        // Locate all existing entities on this grid
        entityList.AddRange(GetComponentsInChildren<Entity>());
        Debug.Log(entityList.Count + " entities found in grid: " + name);

        // Initialize and update all entities on the grid
        foreach (var item in entityList)
        {
            item.Initialize(this);
            item.UpdateEntity();

            if (item.hasCollision)
            {
                SetCollision(item.coordinates, true);
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

    private void OnDrawGizmos()
    {
        if (showGrid) DEBUG_DrawGrid();
        if (showCollisions) DEBUG_DrawCollisions();
    }

    private void DEBUG_DrawGrid()
    {
        Gizmos.color = Color.white;
        for (int x = -gridSize.x; x < gridSize.x; x++)
        {
            for (int y = -gridSize.y; y < gridSize.y; y++)
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
                if (gridCollisions[x,y])
                {
                    Vector2Int coordinates = new Vector2Int(x - gridSize.x, y - gridSize.y);
                    Gizmos.DrawWireCube(GridToWorld(coordinates, CELL_SIZE / 2), new Vector3(CELL_SIZE, CELL_SIZE, CELL_SIZE));
                }
            }
        }
    }
#endif
}
