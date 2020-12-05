using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectList : MonoBehaviour
{
    public RectTransform listRectTransform;

    private List<WorldObjectItem> listItems;

    public void Initalize(List<GameObject> worldEditorObjects)
    {
        listItems = new List<WorldObjectItem>();

        for (int i = 0; i < worldEditorObjects.Count - 2; i++)
        {
            GameObject newObject = (GameObject)Instantiate(Resources.Load("Prefabs/UI/WorldObjectItem"));
            WorldObjectItem item = newObject.GetComponent<WorldObjectItem>();
            item.transform.SetParent(listRectTransform.transform);
            item.Initialize(i, worldEditorObjects[i + 2]);
            listItems.Add(item);
        }

        listRectTransform.sizeDelta = new Vector2(0, 20 + (65 * (worldEditorObjects.Count - 2)));
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
