using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mirror;

public class WorldEditor : MonoBehaviour
{
    // Singleton
    public static WorldEditor WorldEditorManager { get; private set; }

    public WorldObjectList objectListUI;
    public Grid attachedGrid;

    public bool IsWorldEditorActive { get; private set; }
    public NetworkManager netManager;

    //world editor button
    public Button worldEditorButton;

    private GameObject[] worldEditorObjects;
    private bool isInitialized = false;


    public void CreateItem(int index)
    {
        GameObject newObject = Instantiate(netManager.spawnPrefabs[index+2]);
        NetworkServer.Spawn(newObject);
        newObject.GetComponent<Draggable>().InitializeInDragMode();
    }

    void showWorldEditor(){
        // World edit mode change not allowed while something is selected
        if (!Draggable.HasSelection)
        {
            IsWorldEditorActive = !IsWorldEditorActive;
            objectListUI.gameObject.SetActive(IsWorldEditorActive);

            if (IsWorldEditorActive && !isInitialized)
            {
                objectListUI.Initalize(netManager.spawnPrefabs);
                isInitialized = true;
            }
        }
    }


    // Use this for initialization
    void Start()
    {
        IsWorldEditorActive = false;
        worldEditorObjects = Resources.LoadAll<GameObject>("Prefabs/Environment");

        if (WorldEditorManager == null)
            WorldEditorManager = this;
        
        worldEditorButton.onClick.AddListener(showWorldEditor);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            showWorldEditor();
        }

    }
}
