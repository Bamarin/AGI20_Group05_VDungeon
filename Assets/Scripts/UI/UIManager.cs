using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //needs to update according to the network manager
    public int numOfCurrentCharacter = 0;
    public GameObject FPSGuide;
    private bool ifRightClicked = false;
    public GameObject DiceResultLabel;
    private TMP_Text resultText;
    private Animator bgRoll;
    public Button rollButton;

    private bool isRolled = false;
    //public DiceController diceController;

    IEnumerator HideFPSGuide()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);
        FPSGuide.GetComponent<Animator>().SetTrigger("close");
    }

    private void detectFirstRoll(){
        isRolled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        FPSGuide.SetActive(true);
        FPSGuide.GetComponent<Animator>().SetTrigger("pop");

        //set dice result label to the right position
        DiceResultLabel.GetComponent<RectTransform>().anchorMin = new Vector2(0.18f * numOfCurrentCharacter + 0.16f, 0.8f);
        DiceResultLabel.GetComponent<RectTransform>().anchorMax = new Vector2(0.18f * numOfCurrentCharacter + 0.16f, 0.8f);

        resultText = DiceResultLabel.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        bgRoll = DiceResultLabel.transform.GetChild(0).gameObject.GetComponent<Animator>();

        rollButton.onClick.AddListener(detectFirstRoll);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ifRightClicked && Input.GetMouseButton(1))
        {
            ifRightClicked = true;
            StartCoroutine(HideFPSGuide());
        }

        if (isRolled)
        {
            if (DiceController.diceVelocity == new Vector3(0f, 0f, 0f) && DiceController.diceNumber != 0)
            {
                resultText.text = DiceController.diceNumber.ToString();
                bgRoll.SetTrigger("normal");
            }
            else if (DiceController.diceVelocity != new Vector3(0f, 0f, 0f) && DiceController.diceNumber == 0)
            {
                resultText.text = "?";
                bgRoll.SetTrigger("roll");
            }
        }
    }
}
