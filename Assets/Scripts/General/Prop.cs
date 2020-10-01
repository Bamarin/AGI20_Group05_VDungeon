using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An Entity whose position and orientation are regulated by a Grid
public class Prop : Entity
{

    // *** PROPERTY FIELDS ***

    public Grid.Orientation orientation;


    // *** INTERNAL VARIABLES ***

    private int randomRotationValue;


    // *** UTILITY FUNCTIONS ***

    // Rotates the prop towards a new grid orientation.
    public void Rotate(Grid.Orientation newOrientation)
    {
        orientation = newOrientation;
        UpdateOrientation();
    }

    // Updates the prop's rotation to match its current grid orientation.
    public virtual void UpdateOrientation()
    {
        transform.localEulerAngles = new Vector3(0, OrientationAngle());
    }

    // Ensures the prop is correctly positioned and oriented within the grid.
    public override void UpdateEntity()
    {
        base.UpdateEntity();
        UpdateOrientation();
    }

    protected float OrientationAngle()
    {
        if (orientation == Grid.Orientation.Random)
            return randomRotationValue;

        return Grid.OrientationToAngle(orientation);
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    private void Awake()
    {
        // Load a consistent value for random rotation for this object
        randomRotationValue = Random.Range(0, 360);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
