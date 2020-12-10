using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 diceVelocity;
    public static int diceNumber;
    public float cameraMoveSpeed = 2f;
    Vector3 initDicePos;
    Vector3 initCameraPos;
    GameObject diceView;
    float cameraDistance;
    public float cameraDisplacement = 2f;

    public Button diceRollButton;

    //roll dice button
    void rollDice(){
        diceView.transform.position = initCameraPos;
        diceNumber = 0;
        float dirX = Random.Range(0, 500);
        float dirY = Random.Range(0, 500);
        float dirZ = Random.Range(0, 500);
        transform.position = new Vector3(initDicePos.x, initCameraPos.y-3, initDicePos.z);
        transform.rotation = Quaternion.identity;
        rb.AddForce(transform.up * 500);
        rb.AddTorque(dirX, dirY, dirZ);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initDicePos = this.transform.position;
        diceView = GameObject.Find("DiceView");
        initCameraPos = diceView.transform.position;
        cameraDistance = initCameraPos.y - initDicePos.y - cameraDisplacement;

        diceRollButton.onClick.AddListener(rollDice);
    }

    // Update is called once per frame
    void Update()
    {
        diceVelocity = rb.velocity;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rollDice();
        }
        if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        {
            diceView.transform.position=Vector3.Lerp(diceView.transform.position, new Vector3(this.transform.position.x, this.transform.position.y + cameraDistance, this.transform.position.z), Time.deltaTime * cameraMoveSpeed);
        }
    }

}
