using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGridManager : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    // Reasoning for this value: considering 1 unit = 1 meter and the default DnD cell size of 5 feet =~ 1.5 meters
    // However this could be made configurable in the future
    public const float CELL_SIZE = 1.5f;

    // Stores all objects attached to this grid
    private List<GridObject> gridObjects;


    // *** UTILITY FUNCTIONS ***

    // Convert grid position to local position. (Y axis is set to zero)
    public static Vector3 GridToLocal(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * CELL_SIZE, 0, gridPosition.y * CELL_SIZE);
    }

    // Convert local position to grid position. (ignores Y axis)
    public static Vector2Int LocalToGrid(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt((position.x + (CELL_SIZE / 2)) / CELL_SIZE), Mathf.FloorToInt((position.z + (CELL_SIZE / 2)) / CELL_SIZE));
    }

    // Convert grid position to world position. (Y axis is set to the grid's origin)
    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return transform.TransformPoint(GridToLocal(gridPosition));
    }

    // Convert world position to grid position. (ignores Y axis)
    public Vector2Int WorldToGrid(Vector3 position)
    {
        return LocalToGrid(transform.InverseTransformPoint(position));
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        gridObjects = new List<GridObject>();

        // Locate all existing grid objects
        gridObjects.AddRange(GetComponentsInChildren<GridObject>());
        Debug.Log(gridObjects.Count + " grid objects found in " + name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
