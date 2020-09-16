using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GridObject))]
public class GridObjectEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GridObject castedTarget = (GridObject)target;

        EditorGUI.BeginChangeCheck();

        Vector2Int newGridPosition = EditorGUILayout.Vector2IntField("Grid Position: ", castedTarget.gridPosition);
        if (newGridPosition != castedTarget.gridPosition)
        {
            castedTarget.MoveToGridPosition(newGridPosition);
        }

        if (GUILayout.Button("Snap to Grid"))
        {
            castedTarget.SnapToLocal();
        }

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            EditorUtility.SetDirty(castedTarget);
            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
