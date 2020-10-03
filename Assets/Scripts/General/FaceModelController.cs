using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FaceModelController : MonoBehaviour
{
    public GameObject head;
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject mouth;

    public static Vector3 headPos;
    public static Quaternion headRot;
    public static Vector3 leftEyeShape;
    public static Vector3 rightEyeShape;
    public static Vector3 mouthShape;

    private string data;
    private string[] dataArray;

    void Start()
    {
        //Initialization of model parameters
        headPos = head.transform.position;
        headRot = head.transform.rotation;
        leftEyeShape = leftEye.transform.localScale;
        rightEyeShape = rightEye.transform.localScale;
        mouthShape = mouth.transform.localScale;
       
    }
    // Update is called once per frame
    void Update()
    {
        data = SocketClient.dataString;
        
        if (!String.IsNullOrEmpty(data))
        {
            dataArray = data.Split(':');

            headRot.w = Convert.ToSingle(dataArray[3]);
            headRot.x = Convert.ToSingle(dataArray[4]);
            headRot.y = Convert.ToSingle(dataArray[5]);
            headRot.z = Convert.ToSingle(dataArray[6]);

            headRot.w = Kalman_filter(headRot.w, 8e-3f, 5e-4f);
            headRot.x = Kalman_filter(headRot.x, 8e-3f, 5e-4f);
            headRot.y = Kalman_filter(headRot.y, 8e-3f, 5e-4f);
            headRot.z = Kalman_filter(headRot.z, 8e-3f, 5e-4f);

            leftEyeShape = new Vector3(0.2f, Convert.ToSingle(dataArray[7]), 0.2f);
            if (leftEyeShape[1] < 0.12f)
            {
                leftEyeShape[1] = 0.01f;
            }

            rightEyeShape = new Vector3(0.2f, Convert.ToSingle(dataArray[8]), 0.2f);
            if (rightEyeShape[1] < 0.12f)
            {
                rightEyeShape[1] = 0.01f;
            }

            mouthShape = new Vector3(Convert.ToSingle(dataArray[10]), Convert.ToSingle(dataArray[9]), 0.2f);

            //Apply rotation changes
            head.transform.rotation = headRot;

            //Apply facial expression shapes changes
            leftEye.transform.localScale = leftEyeShape;
            rightEye.transform.localScale = rightEyeShape;
            mouth.transform.localScale = mouthShape;
        }
       
    }

    public float Kalman_filter(float input, float Q, float R)
    {
        float K;
        float X = 0;
        float P = 0.1f;
        K = P / (P + R);
        X = X + K * (input - X);
        P = P - K * P + Q;
        return X;
    }

}
