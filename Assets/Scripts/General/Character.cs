using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    // *** PROPERTY FIELDS ***

    public float mouseSensitivity = 3f;
    public GameObject playerHead;

    //first person view camera
    public GameObject ViewCamera;
    //the camera towards the character's face
    public GameObject TowardsCamera;
    //for the adjusting the the y of towards camera to force it face the character's face
    public float headHight = 0.8f;
    public int numOfCurrentCharacter = 0;


    // *** PROPERTY FIELDS ***

    // The default material to render this character with when it is not highlighted
    public Material defaultMaterial;
    public float rotateSpeed = 40f;
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
        Vector3 playerRotation = transform.rotation.eulerAngles;
        // up-and-down for character's head
        //headRotation.x -= rotationY;
        //headRotation.z = 0;

        // left-and-right for character
        playerRotation.y += rotationX;

        transform.rotation = Quaternion.Euler(playerRotation);
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    private void Awake()
    {

    }

    public override void OnStartLocalPlayer()
    {
        ViewCamera = new GameObject("View Camera", typeof(CameraView));
        ViewCamera.AddComponent<Camera>();
        ViewCamera.GetComponent<Camera>().rect = new Rect(0.7f, 0, 0.3f, 0.8f);
        ViewCamera.GetComponent<CameraView>().player = gameObject.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        TowardsCamera = new GameObject("Toward Camera", typeof(CameraTowards));
        TowardsCamera.AddComponent<Camera>();
        TowardsCamera.GetComponent<Camera>().rect = new Rect(numOfCurrentCharacter * 0.17f, 0.8f, 0.15f, 0.2f);
        CameraTowards ct = TowardsCamera.GetComponent<CameraTowards>();
        ct.player = gameObject.transform;
        ct.offset.y = headHight;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            return;
        }

        if (!SystemInfo.supportsGyroscope)
        {
            //enable mouse to control the first person view when the gyroscope is not avaliable
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                // enable fps when the right click is holding
                FPScontrol();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }


    }
}