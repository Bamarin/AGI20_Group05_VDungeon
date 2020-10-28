using UnityEngine;
using System.Collections;

public class WorldEditor : MonoBehaviour
{
    public WorldObjectList objectListUI;

    public bool IsWorldEditorActive { get; private set; }

    private GameObject[] worldEditorObjects;
    private bool isInitialized = false;


    // Use this for initialization
    void Start()
    {
        IsWorldEditorActive = false;
        worldEditorObjects = Resources.LoadAll<GameObject>("Prefabs/Environment");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsWorldEditorActive = !IsWorldEditorActive;
            objectListUI.gameObject.SetActive(IsWorldEditorActive);

            if (IsWorldEditorActive && !isInitialized)
            {
                objectListUI.Initalize(worldEditorObjects);
                isInitialized = true;
            }
        }
    }
}
