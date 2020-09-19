using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointAttching : MonoBehaviour
{
    [Header("Grid Parameters")]
    [Range(1f, 10f)]
    public float gridSize = 1f; 
    public Vector2Int gridNumber = new Vector2Int(10, 10);
    public Vector2 leftBottomPosition = new Vector2(-5f, -5f);
    
    // Start is called before the first frame update
    void Start()
    {
        //create grid points
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
