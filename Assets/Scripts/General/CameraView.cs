using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform player;
    public Character mouse; //only for getting the same mouse sensitivity now

    public float yOffset = 0.85f; // move to the height of the face

    [Header("Mobile Control")]
    public bool gyroscopeControl = true;
    private Gyroscope gyro;
    //to check whether the gyroscope is avaliable on the mobile
    private bool ifGyroEnabled;

    private GameObject firstView;
    private Quaternion rotMatrix;
    //for swipe rotation
    private Touch initialTouch = new Touch();
    private Vector3 originalRotation;
    private float swipeRotateX;
    private float swipeRotateY;
    public float swipeSpeed = 0.5f;

    //for FPS control & swipe control
    private float clampUpAndDown;

    bool EnableGryo()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            if (gyroscopeControl)
            {
                firstView.transform.rotation = Quaternion.Euler(90f, 90f, 0f); //pointing towards
            }
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

        originalRotation = transform.eulerAngles;
        swipeRotateX = originalRotation.x;
        swipeRotateY = originalRotation.y;

        clampUpAndDown = 0;
    }

    private void Update()
    {
        if (ifGyroEnabled)
        {//to avoid non-reference error
            firstView.transform.position = player.position + new Vector3(0f, yOffset, 0f);
            if (gyroscopeControl)
            {
                transform.localRotation = gyro.attitude * rotMatrix;
            }
            else
            {
                // swipe rotation
                foreach (Touch touch in Input.touches)
                { //enable multiple touch
                    if (touch.phase == TouchPhase.Began)
                    {
                        initialTouch = touch;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {

                        //swipeRotateX -= (touch.position.y - initialTouch.position.y) * Time.deltaTime * swipeSpeed;
                        //swipeRotateY += (touch.position.x - initialTouch.position.x) * Time.deltaTime * swipeSpeed;
                        //transform.eulerAngles = new Vector3(swipeRotateX, swipeRotateY, 0f);
                        //Debug.Log(touch.position.y);

                        Vector3 swipeRotation = transform.rotation.eulerAngles;
                        // up-and-down
                        swipeRotation.x -= (touch.position.y - initialTouch.position.y) * Time.deltaTime * swipeSpeed;
                        swipeRotation.z = 0;

                        clampUpAndDown -= (touch.position.y - initialTouch.position.y) * Time.deltaTime * swipeSpeed;

                        //limit the up-and-down rotationn
                        if (clampUpAndDown > 40)
                        {
                            clampUpAndDown = 40;
                            swipeRotation.x = 40;
                        }
                        else if (clampUpAndDown < -40)
                        {
                            clampUpAndDown = -40;
                            swipeRotation.x = -40;
                        }

                        // left-and-right
                        swipeRotation.y += (touch.position.x - initialTouch.position.x) * Time.deltaTime * swipeSpeed;
                        transform.localRotation = Quaternion.Euler(swipeRotation);
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        initialTouch = new Touch();
                    }
                }
            }



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
