using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    public Vector2Int gridPosition;


    // *** UTILITY FUNCTIONS ***

    // Moves this object to a new grid position
    public void MoveToGridPosition(Vector2Int position)
    {
        gridPosition = position;
        SnapToGrid();
    }

    // Updates this object's local position to match its grid position
    public void SnapToGrid()
    {
        transform.localPosition = WorldGridManager.GridToLocal(gridPosition);
    }

    // Updates this object's grid position to match its local position. (moves object to nearest cell)
    public void SnapToLocal()
    {
        MoveToGridPosition(WorldGridManager.LocalToGrid(transform.localPosition));
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
