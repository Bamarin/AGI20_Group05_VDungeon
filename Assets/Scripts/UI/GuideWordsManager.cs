using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideWordsManager : MonoBehaviour
{
    public GameObject FPSGuide;
    private bool ifRightClicked = false;

    IEnumerator HideFPSGuide()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);
        FPSGuide.GetComponent<Animator>().SetTrigger("close");

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    // Start is called before the first frame update
    void Start()
    {
        FPSGuide.SetActive(true);
        FPSGuide.GetComponent<Animator>().SetTrigger("pop");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ifRightClicked && Input.GetMouseButton(1)){
            ifRightClicked = true;
            StartCoroutine(HideFPSGuide());
        }
    }
}
