using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCollisionChecker : MonoBehaviour
{
    Vector3 diceVelocity;
    // Start is called before the first frame update
    void FixedUpdate()
    {
        diceVelocity = DiceController.diceVelocity;   
    }

    void OnTriggerStay(Collider other)
    {
        if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        {
            DiceController.diceNumber = 21 - int.Parse(other.gameObject.name);
            print(DiceController.diceNumber);
        }
    }
}
