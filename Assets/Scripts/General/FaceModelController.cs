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

    // Kalman objects
    KalmanObject W_Kalman = new KalmanObject();
    KalmanObject X_Kalman = new KalmanObject();
    KalmanObject Y_Kalman = new KalmanObject();
    KalmanObject Z_Kalman = new KalmanObject();

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

            headRot.w = W_Kalman.Kalman_filter(headRot.w, 8e-3f, 5e-4f);
            headRot.x = X_Kalman.Kalman_filter(headRot.x, 8e-3f, 5e-4f);
            headRot.y = Y_Kalman.Kalman_filter(headRot.y, 8e-3f, 5e-4f);
            headRot.z = Z_Kalman.Kalman_filter(headRot.z, 8e-3f, 5e-4f);

            //leftEyeShape = leftEye.transform.localScale;
            //rightEyeShape = rightEye.transform.localScale;
            //mouthShape = mouth.transform.localScale;

            //Apply rotation changes
            head.transform.rotation = headRot;
            Debug.Log(headRot);
            //Apply facial expression shapes changes
            leftEye.transform.localScale = leftEyeShape;
            rightEye.transform.localScale = rightEyeShape;
            mouth.transform.localScale = mouthShape;
        }
       
    }

    class KalmanObject
    {
        public float K;
        public float X = 0;
        public float P = 0.1f;

        public float Kalman_filter(float input, float Q, float R)
        {
            K = P / (P + R);
            X = X + K * (input - X);
            P = P - K * P + Q;
            return X;
        }
    }
}
