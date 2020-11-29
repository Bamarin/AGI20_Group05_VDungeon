using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager diceManager { get; private set; }
    public bool IsDiceTurn { get; private set; }
    public GameObject DiceBoard;
    // Start is called before the first frame update
    void Start()
    {
        IsDiceTurn = false;
        if (diceManager == null)
        {
            diceManager = this;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            IsDiceTurn = !IsDiceTurn;
            DiceBoard.SetActive(IsDiceTurn);
        }
    }
}
