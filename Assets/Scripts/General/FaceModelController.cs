using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class FaceModelController : MonoBehaviour
{
    public GameObject head;

    public static Vector3 headPos;
    public static Quaternion headRot;
    public static Vector3 leftEyeShape;
    public static Vector3 rightEyeShape;
    public static Vector3 mouthShape;

    private string data;
    private string[] dataArray;

    SkinnedMeshRenderer face;
    const int LRaiseBrow = 0;
    const int LAngryBrow = 1;
    const int RRaiseBrow = 2;
    const int RAngryBrow = 3;
    const int LCloseEye = 4;
    const int RCloseEye = 5;
    const int MouthWidth = 6;
    const int MouthHeight = 7;

    private float LRaiseBrowWeight;
    private float LAngryBrowWeight;
    private float RRaiseBrowWeight;
    private float RAngryBrowWeight;
    private float LCloseEyeWeight;
    private float RCloseEyeWeight;
    private float MouthWidthWeight;
    private float MouthHeightWeight;
    private float shockEyeWeight;

    void Start()
    {
        //Initialization of model parameters
        headRot = gameObject.transform.parent.rotation;
        face = GetComponent<SkinnedMeshRenderer>();
        headRot.w = 0;
        headRot.x = 0.5f;
        headRot.y = 0;
        headRot.z = 1;
       
    }
    // Update is called once per frame
    void Update()
    {
        data = SocketClient.dataString;
        
        if (!String.IsNullOrEmpty(data))
        {
            dataArray = data.Split(':');
           
            headRot.w = Single.Parse(dataArray[3], CultureInfo.InvariantCulture);
            headRot.x = Single.Parse(dataArray[4], CultureInfo.InvariantCulture);
            headRot.y = Single.Parse(dataArray[5], CultureInfo.InvariantCulture);
            headRot.z = Single.Parse(dataArray[6], CultureInfo.InvariantCulture);

            headRot.w = Kalman_filter(headRot.w, 8e-3f, 5e-4f);
            headRot.x = Kalman_filter(headRot.x, 8e-3f, 5e-4f);
            headRot.y = Kalman_filter(headRot.y, 8e-3f, 5e-4f);
            headRot.z = Kalman_filter(headRot.z, 8e-3f, 5e-4f);

            float eyeMax = 0.27f;
            float eyeMin = 0.15f;

            // Left Eye closeness
            leftEyeShape = new Vector3(0.2f, Single.Parse(dataArray[7], CultureInfo.InvariantCulture), 0.2f);
            LCloseEyeWeight = Mathf.Lerp(0, 100, Mathf.InverseLerp(eyeMax, eyeMin, leftEyeShape[1]));

            // Right Eye closeness
            rightEyeShape = new Vector3(0.2f, Single.Parse(dataArray[8], CultureInfo.InvariantCulture), 0.2f);
            RCloseEyeWeight = Mathf.Lerp(0, 100, Mathf.InverseLerp(eyeMax, eyeMin, rightEyeShape[1]));

            mouthShape = new Vector3(Single.Parse(dataArray[10], CultureInfo.InvariantCulture), Single.Parse(dataArray[9], CultureInfo.InvariantCulture), 0.2f);

            if (500 * mouthShape[1] < 100f)
            {
                MouthHeightWeight = 500 * mouthShape[1];
            }
            else
            {
                MouthHeightWeight = 100f;
            }

            //Apply rotation changes
            head.transform.parent.localRotation = headRot;

            //Apply facial expression shapes changes
            face.SetBlendShapeWeight(LCloseEye, LCloseEyeWeight);
            face.SetBlendShapeWeight(RCloseEye, RCloseEyeWeight);
            face.SetBlendShapeWeight(MouthHeight, MouthHeightWeight);
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
