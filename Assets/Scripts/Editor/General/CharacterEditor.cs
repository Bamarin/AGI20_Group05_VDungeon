using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Character))]
public class CharacterEditor : EntityEditor
{

    public override void OnInspectorGUI()
    {
        Character castedTarget = (Character)target;

        EditorGUI.BeginChangeCheck();

        CreateCharacterGUI(castedTarget);
        CreateCoordinatesGUI(castedTarget);

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            castedTarget.UpdateEntity();

            EditorUtility.SetDirty(castedTarget);
            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected void CreateCharacterGUI(Character castedTarget)
    {
        GUILayout.Label("Character", EditorStyles.boldLabel);

        castedTarget.interactable = EditorGUILayout.Toggle("Interactable", castedTarget.interactable);
        castedTarget.rotateSpeed = EditorGUILayout.Slider("Rotate Speed", castedTarget.rotateSpeed, 10f, 50f);

        castedTarget.defaultMaterial = (Material)EditorGUILayout.ObjectField("Default Material", castedTarget.defaultMaterial, typeof(Material), false);
    }
}
