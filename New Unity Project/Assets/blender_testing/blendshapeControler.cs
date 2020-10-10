using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blendshapeControler : MonoBehaviour
{
    // Start is called before the first frame update
    SkinnedMeshRenderer cube;
    void Start()
    {
        cube = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float cur = cube.GetBlendShapeWeight(0) + 1;
        cur = cur % 101;
        cube.SetBlendShapeWeight(0, cur);
        cube.SetBlendShapeWeight(1, 100 - cur);
        
    }
}
