using UnityEngine;
using System.Collections;

public class WorldEditor : MonoBehaviour
{
    // Singleton
    public static WorldEditor WorldEditorManager { get; private set; }

    public WorldObjectList objectListUI;
    public Grid attachedGrid;

    public bool IsWorldEditorActive { get; private set; }

    private GameObject[] worldEditorObjects;
    private bool isInitialized = false;


    public void CreateItem(int index)
    {
        GameObject newObject = Instantiate(worldEditorObjects[index].gameObject);
        newObject.GetComponent<Entity>().Initialize(attachedGrid);
        newObject.GetComponent<Draggable>().SetPlacementMode();
    }


    // Use this for initialization
    void Start()
    {
        IsWorldEditorActive = false;
        worldEditorObjects = Resources.LoadAll<GameObject>("Prefabs/Environment");

        if (WorldEditorManager == null)
            WorldEditorManager = this;
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
