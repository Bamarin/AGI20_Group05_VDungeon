using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Prop))]
public class PropEditor : EntityEditor
{

    public override void OnInspectorGUI()
    {
        Prop castedTarget = (Prop)target;

        EditorGUI.BeginChangeCheck();

        CreateCoordinatesGUI(castedTarget);
        CreateOrientationGUI(castedTarget);  

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            castedTarget.UpdateEntity();

            EditorUtility.SetDirty(castedTarget);
            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected void CreateOrientationGUI(Prop castedTarget)
    {
        GUILayout.Label("Grid Orientation", EditorStyles.boldLabel);

        castedTarget.orientation = (Grid.Orientation)EditorGUILayout.EnumPopup("Orientation: ", castedTarget.orientation);
    }
}
