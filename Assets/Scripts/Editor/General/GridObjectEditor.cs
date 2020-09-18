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

        // GRID POSITION
        GUILayout.Label("Grid Position", EditorStyles.boldLabel);

        castedTarget.gridPosition = EditorGUILayout.Vector2IntField("Coordinates: ", castedTarget.gridPosition);

        if (GUILayout.Button(new GUIContent("North", "Move this grid object one step North.")))
        {
            castedTarget.gridPosition = castedTarget.gridPosition + Vector2Int.up;
        }
        if (GUILayout.Button(new GUIContent("South", "Move this grid object one step South.")))
        {
            castedTarget.gridPosition = castedTarget.gridPosition + Vector2Int.down;
        }
        if (GUILayout.Button(new GUIContent("East", "Move this grid object one step East.")))
        {
            castedTarget.gridPosition = castedTarget.gridPosition + Vector2Int.right;
        }
        if (GUILayout.Button(new GUIContent("West", "Move this grid object one step West.")))
        {
            castedTarget.gridPosition = castedTarget.gridPosition + Vector2Int.left;
        }
        if (GUILayout.Button(new GUIContent("Snap to Grid", "Move this grid object to the nearest grid cell based on its current position.")))
        {
            castedTarget.SnapToLocal();
        }

        // GRID ORIENTATION
        GUILayout.Label("Grid Orientation", EditorStyles.boldLabel);

        castedTarget.gridOrientation = (GridObject.Orientation)EditorGUILayout.EnumPopup("Orientation: ", castedTarget.gridOrientation);

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            castedTarget.SnapToAll();

            EditorUtility.SetDirty(castedTarget);
            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
