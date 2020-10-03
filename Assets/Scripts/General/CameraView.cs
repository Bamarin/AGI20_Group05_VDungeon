using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform player;
    public Character mouse; //only for getting the same mouse sensitivity now

    public float yOffset = 0.85f; // move to the height of the face

    private Gyroscope gyro;
    //to check whether the gyroscope is avaliable on the mobile
    private bool ifGyroEnabled;

    private GameObject firstView;
    private Quaternion rotMatrix;

    private float clampUpAndDown;

    bool EnableGryo()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            firstView.transform.rotation = Quaternion.Euler(90f, 90f, 0f); //pointing towards
            return true;
        }
        else
        {
            Debug.Log("no gryoscope is valid in the mobile");
            return false;
        }
    }

    private void Start()
    {
        firstView = new GameObject("First View");
        firstView.transform.position = transform.position;
        transform.SetParent(firstView.transform);

        ifGyroEnabled = EnableGryo();

        rotMatrix = new Quaternion(0, 0, 1, 0);

        clampUpAndDown = 0;
    }

    private void Update()
    {
        //firstView.transform.rotation = player.rotation;
        if (ifGyroEnabled)
        {//to avoid non-reference error
            firstView.transform.position = player.position + new Vector3(0f, yOffset, 0f);
            transform.localRotation = gyro.attitude * rotMatrix;
        }
    }


    private void LateUpdate()
    {
        if (!ifGyroEnabled)
        {
            firstView.transform.position = player.position + new Vector3(0f, yOffset, 0f);
            // only trigger the first-view rotation change when right clicked the mouse
            if (Input.GetMouseButton(1))
            {
                //the following could be changed to sychronizing the character when the head rotation could be caught
                // change per frame
                float rotationX = Input.GetAxis("Mouse X") * mouse.mouseSensitivity;
                float rotationY = Input.GetAxis("Mouse Y") * mouse.mouseSensitivity;

                clampUpAndDown -= rotationY; //keep for relative rotation

                Vector3 camRotation = transform.rotation.eulerAngles;
                // up-and-down
                camRotation.x -= rotationY;
                camRotation.z = 0;

                //limit the up-and-down rotationn
                if (clampUpAndDown > 40)
                {
                    clampUpAndDown = 40;
                    camRotation.x = 40;
                }
                else if (clampUpAndDown < -40)
                {
                    clampUpAndDown = -40;
                    camRotation.x = -40;
                }

                // left-and-right
                camRotation.y += rotationX;
                transform.rotation = Quaternion.Euler(camRotation);
            }
        }
    }

}
