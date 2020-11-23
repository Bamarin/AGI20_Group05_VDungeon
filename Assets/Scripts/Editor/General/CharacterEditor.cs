//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;

//[CustomEditor(typeof(Character))]
//public class CharacterEditor : EntityEditor
//{

//    public override void OnInspectorGUI()
//    {
//        Character castedTarget = (Character)target;

//        EditorGUI.BeginChangeCheck();

//        CreateCharacterGUI(castedTarget);
//        CreateCollisionGUI(castedTarget);
//        CreateCoordinatesGUI(castedTarget);

//        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
//        {
//            castedTarget.UpdateEntity();

//            EditorUtility.SetDirty(castedTarget);
//            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
//        }

//        serializedObject.ApplyModifiedProperties();
//    }

//    protected void CreateCharacterGUI(Character castedTarget)
//    {
//        GUILayout.Label("Character", EditorStyles.boldLabel);

//        castedTarget.activeCharacter = EditorGUILayout.Toggle("Active Character", castedTarget.activeCharacter);
//        castedTarget.mouseSensitivity = EditorGUILayout.Slider("Mouse Sensitivity", castedTarget.mouseSensitivity, 1f, 10f);
//        //for control of hear look up and down
//        castedTarget.playerHead = (GameObject)EditorGUILayout.ObjectField("Head Object", castedTarget.playerHead, typeof(GameObject), true);
//        //tempararyly for number of current character
//        castedTarget.numOfCurrentCharacter = EditorGUILayout.IntField("Character Number (Not inclusive)", castedTarget.numOfCurrentCharacter);
//    }
//}
