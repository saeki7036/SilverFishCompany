using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridContent))]
public class GridContentFieldViewer : Editor
{
    SerializedProperty itemInfoProperty;

    void OnEnable()
    {
        itemInfoProperty = serializedObject.FindProperty("content.item");
    }

    public override void OnInspectorGUI()
    {
        // 更新
        serializedObject.Update();

        //base.OnInspectorGUI();

        var contentProp = serializedObject.FindProperty("content");
        EditorGUILayout.PropertyField(contentProp);

        GridContent instance = target as GridContent;

        // GridCellType が条件を満たすときのみ、[ItemInformation]のフィールド変数を表示する
        if (instance.Content.GridCellType == BuildType.Production || 
            instance.Content.GridCellType == BuildType.Processing)
        {
            EditorGUILayout.PropertyField(itemInfoProperty, new GUIContent("Item Info"));
        }

        // 変更を反映
        serializedObject.ApplyModifiedProperties();
    }
}
