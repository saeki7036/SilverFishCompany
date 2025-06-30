using UnityEngine;
using UnityEditor;

/// <summary>
/// GridContentクラス用のカスタムエディター
/// Inspector上でGridCellTypeに応じてアイテム情報フィールドの表示/非表示を制御する
/// </summary>
[CustomEditor(typeof(GridContent))]
public class GridContentFieldViewer : Editor
{
    // アイテム情報プロパティの参照を保持
    SerializedProperty itemInfoProperty;

    /// <summary>
    /// エディター有効化時の初期化処理
    /// 必要なシリアライズプロパティの参照を取得する
    /// </summary>
    void OnEnable()
    {
        // content.itemプロパティの参照を取得
        itemInfoProperty = serializedObject.FindProperty("content.item");
    }

    /// <summary>
    /// Inspector GUI描画処理
    /// GridCellTypeの値に応じて表示するフィールドを動的に変更する
    /// </summary>
    public override void OnInspectorGUI()
    {
        // 更新
        serializedObject.Update();

        // contentプロパティを表示
        var contentProp = serializedObject.FindProperty("content");
        EditorGUILayout.PropertyField(contentProp);

        // ターゲットオブジェクトの取得
        GridContent instance = target as GridContent;

        // GridCellType が条件を満たすときのみ、[ItemInformation]のフィールド変数を表示する
        // Production（生産）またはProcessing（加工）の場合にアイテム情報を表示
        if (instance.GetContent().GridCellType == BuildType.Production || 
            instance.GetContent().GridCellType == BuildType.Processing)
        {
            // アイテム情報フィールドを表示（ラベルを"Item Info"に変更）
            EditorGUILayout.PropertyField(itemInfoProperty, new GUIContent("Item Info"));
        }

        // 変更を反映
        serializedObject.ApplyModifiedProperties();
    }
}
