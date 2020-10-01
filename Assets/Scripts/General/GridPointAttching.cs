using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointAttching : MonoBehaviour
{
    [Header("Grid Parameters")]
    [Range(1f, 10f)]
    public float gridSize = 1f; 
    public Vector2Int gridNumber = new Vector2Int(11, 11);
    public Vector2 leftBottomPosition = new Vector2(-5f, -5f);

    //for the material change to show the nearest grid point
    public Material oriMat;
    public Material nearMat;


    public List<GameObject> gridVertices;
    
    // Start is called before the first frame update
    void Start()
    {
        //create grid points
        for (int j = 0; j < gridNumber.y; j++){
            for (int i = 0; i < gridNumber.x; i++){
                GameObject vertice = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                vertice.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                vertice.transform.position = new Vector3(leftBottomPosition.x + i*gridSize, 
                    0.1f, leftBottomPosition.y + j*gridSize);
                vertice.GetComponent<MeshRenderer>().material = oriMat;
                gridVertices.Add(vertice);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
