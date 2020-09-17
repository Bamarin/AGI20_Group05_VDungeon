using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GridWall))]
public class GridWallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridWall castedTarget = (GridWall)target;

        bool needsUpdate = false;

        EditorGUI.BeginChangeCheck();

        Vector2Int newGridPosition = EditorGUILayout.Vector2IntField("Grid Position: ", castedTarget.gridPosition);
        if (newGridPosition != castedTarget.gridPosition)
        {
            needsUpdate = true;
            castedTarget.gridPosition = newGridPosition;
        }

        GridObject.Orientation newOrientation = (GridObject.Orientation)EditorGUILayout.EnumPopup("Grid Orientation: ", castedTarget.gridOrientation);
        if (newOrientation != castedTarget.gridOrientation)
        {
            needsUpdate = true;
            castedTarget.gridOrientation = newOrientation;
        }

        bool newIsCorner = EditorGUILayout.Toggle("Is Corner Piece", castedTarget.isCornerPiece);
        if (newIsCorner != castedTarget.isCornerPiece)
        {
            needsUpdate = true;
            castedTarget.isCornerPiece = newIsCorner;
        }

        if (GUILayout.Button("Snap to Grid"))
        {
            needsUpdate = true;
        }

        if (needsUpdate)
        {
            castedTarget.SnapToOrientation();
        }

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            EditorUtility.SetDirty(castedTarget);
            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }
}