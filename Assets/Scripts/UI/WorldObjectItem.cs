using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class WorldObjectItem : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
{
    public int itemIndex;

    private TMP_Text myText;
    private RectTransform rectTransform;


    // *** INITIALIZATION ***

    public void Initialize(int itemIndex, GameObject obj)
    {
        myText = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();

        this.itemIndex = itemIndex;

        // Set text
        myText.text = obj.name;

        // Place item at the right place
        rectTransform.sizeDelta = new Vector2(-20, 30);
        rectTransform.anchoredPosition = new Vector2(15, -10-(itemIndex * 65));
        rectTransform.localScale = new Vector3(1f,1f,1f);
    }


    // *** EVENTS ***
    void OnEnable()
    {
        /*
        if (myText != null)
            myText.color = Color.black;*/
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //myText.color = Color.blue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //myText.color = Color.black;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WorldEditor.WorldEditorManager.CreateItem(itemIndex);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
