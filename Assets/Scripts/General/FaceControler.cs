using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceControler : MonoBehaviour
{
    // get the meshrenderer used to manipulate blendshapes
    SkinnedMeshRenderer face;
    public Quaternion headRot;
    //prename blendshapes for easier use
    const int LRaiseBrow = 0;
    const int LAngryBrow = 1;
    const int RRaiseBrow = 2;
    const int RAngryBrow = 3;
    const int LCloseEye = 4;
    const int RCloseEye = 5;
    const int MouthWidth = 6;
    const int MouthHeight = 7;
    // Start is called before the first frame update
    void Start()
    {
        face = GetComponent<SkinnedMeshRenderer>();
        headRot = gameObject.transform.parent.rotation;

    }

    // takes a list of 8 parameters and changes them one by one, a value of 0 is neutral face and apart from the closeEye values they can be negative as well.
    // the max value 100 and the lowest negative value is different for each and untested.
    void UpdateFace(int[] faceParameters, Quaternion headrotation)
    {
        if(faceParameters.Length != 8)
        {
            print("Eror: face parameter list wrong size");
            return;
        }

        for(int i = 0; i < faceParameters.Length; i++)
        {
            face.SetBlendShapeWeight(i, faceParameters[i]);
        }

        gameObject.transform.parent.rotation = headRot;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            randomizeFaceTest();
        }

    }

    void randomizeFaceTest()
    {
        int[] testFace = new int[8];
        for (int i = 0; i < testFace.Length; i++)
        {
            testFace[i] = Random.Range(-40, 101);
        }
        UpdateFace(testFace, headRot);
    }
}
