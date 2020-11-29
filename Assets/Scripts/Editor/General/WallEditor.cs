//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;

//[CustomEditor(typeof(Wall))]
//public class WallEditor : PropEditor
//{
//    public override void OnInspectorGUI()
//    {
//        Wall castedTarget = (Wall)target;

//        EditorGUI.BeginChangeCheck();

//        CreateWallGUI(castedTarget);
//        CreateCollisionGUI(castedTarget);
//        CreateCoordinatesGUI(castedTarget);
//        CreateOrientationGUI(castedTarget);

//        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
//        {
//            castedTarget.UpdateEntity();

//            EditorUtility.SetDirty(castedTarget);
//            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
//        }

//        serializedObject.ApplyModifiedProperties();
//    }

//    protected void CreateWallGUI(Wall castedTarget)
//    {
//        GUILayout.Label("Grid Wall", EditorStyles.boldLabel);

//        castedTarget.isCornerPiece = EditorGUILayout.Toggle(new GUIContent("Is Corner Piece", "Corner pieces snap to the corner of the cell."), castedTarget.isCornerPiece);
//    }
//}