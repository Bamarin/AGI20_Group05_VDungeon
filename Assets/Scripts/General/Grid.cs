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

    // Stores all objects attached to this grid
    private List<Entity> entityList;


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


    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        entityList = new List<Entity>();

        // Locate all existing entities on this grid
        entityList.AddRange(GetComponentsInChildren<Entity>());
        Debug.Log(entityList.Count + " entities found in grid: " + name);

        // Initialize and update all entities on the grid
        foreach (var item in entityList)
        {
            item.Initialize(this);
            item.UpdateEntity();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
