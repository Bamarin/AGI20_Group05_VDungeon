using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public static DiceManager diceManager { get; private set; }
    public bool IsDiceTurn { get; private set; }
    public GameObject DiceBoard;

    //dice board button
    public Button diceBoardButton;
    public Button diceRollButton;

    void showDiceBoard(){
        IsDiceTurn = !IsDiceTurn;
        DiceBoard.SetActive(IsDiceTurn);

        diceRollButton.gameObject.SetActive(!diceRollButton.gameObject.activeSelf);
    }

    // Start is called before the first frame update
    void Start()
    {
        IsDiceTurn = false;
        if (diceManager == null)
        {
            diceManager = this;
        }

        diceBoardButton.onClick.AddListener(showDiceBoard);
            
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            showDiceBoard();
        }
    }
}
