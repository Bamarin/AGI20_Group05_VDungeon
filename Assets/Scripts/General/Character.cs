using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    // *** PROPERTY FIELDS ***

    public float mouseSensitivity = 3f;
    public GameObject playerHead;


    // Whether this character can be interacted with or not. Use EnableInteraction() to ensure the character's appearance is updated!
    public bool activeCharacter = false;


    // *** INTERNAL FUNCTIONS ***

    // Enable control the first person view through mouse
    private void FPScontrol()
    {
        // change per frame
        float rotationX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotationY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //currently disable the head rotation up and down
        //Vector3 headRotation = playerHead.transform.rotation.eulerAngles;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        // up-and-down for character's head
        //headRotation.x -= rotationY;
        //headRotation.z = 0;

        // left-and-right for character
        playerRotation.y += rotationX;

        //playerHead.rotation = Quaternion.Euler(headRotation);
        transform.rotation = Quaternion.Euler(playerRotation);
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeCharacter)
        {
            if (!SystemInfo.supportsGyroscope)
            {
                //enable mouse to control the first person view when the gyroscope is not avaliable
                if (Input.GetMouseButton(1))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    // enable fps when the right click is holding
                    FPScontrol();
                }
                else{
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
}
