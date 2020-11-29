using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class FaceModelController : MonoBehaviour
{
    public GameObject head;
    public GameObject faceModel;

    public static Vector3 headPos;
    public static Quaternion headRot;
    public static Vector3 leftEyeShape;
    public static Vector3 rightEyeShape;
    public static Vector3 mouthShape;
    public static float leftEyebrowLift;
    public static float rightEyebrowLift;
    public static float leftFrown;
    public static float rightFrown;

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
        face = faceModel.GetComponent<SkinnedMeshRenderer>();
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


            float eyebrowMax = 3.8f;
            float eyebrowMin = 3.3f;
            // Left Eyebrow lift
            leftEyebrowLift = Single.Parse(dataArray[11], CultureInfo.InvariantCulture);
            //print(leftEyebrowLift);
            LRaiseBrowWeight = Mathf.Lerp(0, 100, Mathf.InverseLerp(eyebrowMax, eyebrowMin, 100 * leftEyebrowLift));
            // Right Eyebrow lift
            rightEyebrowLift = Single.Parse(dataArray[12], CultureInfo.InvariantCulture);
            RRaiseBrowWeight = Mathf.Lerp(0, 100, Mathf.InverseLerp(eyebrowMax, eyebrowMin, 100 * rightEyebrowLift));
            //print(rightEyebrowLift);

            float frownMax = 3f;
            float frownMin = 2.8f;
            // Left Eyebrow frown
            leftFrown = Single.Parse(dataArray[13], CultureInfo.InvariantCulture);
            //print(leftFrown);
            LAngryBrowWeight = Mathf.Lerp(0, 100, Mathf.InverseLerp(frownMax, frownMin, 100 * leftFrown));
            // Right Eyebrow frown
            rightFrown = Single.Parse(dataArray[14], CultureInfo.InvariantCulture);
            RAngryBrowWeight = Mathf.Lerp(0, 100, Mathf.InverseLerp(frownMax, frownMin, 100 * rightFrown));

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
            face.SetBlendShapeWeight(LRaiseBrow, LRaiseBrowWeight);
            face.SetBlendShapeWeight(RRaiseBrow, RRaiseBrowWeight);
            face.SetBlendShapeWeight(LAngryBrow, LAngryBrowWeight);
            face.SetBlendShapeWeight(RAngryBrow, RAngryBrowWeight);
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
