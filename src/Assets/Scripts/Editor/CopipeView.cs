using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Unityのエディターが起動されたときに初期化される属性
[InitializeOnLoad]
public class CopipeView : EditorWindow
{
    // メニューにエディタウィンドウを追加（Editor > OriginalEditerPanels > CopipeView）
    [MenuItem("Editor/OriginalEditerPanels/CopipeView")]
    public static void ShowWindow()
    {
        // ウィンドウを開く（既に開いていればフォーカスする）
        GetWindow<CopipeView>("CopipeView");
    }

    // 配置するGameObject
    private GameObject targetPrefab;

    // 基準地点にも配置するかどうかのフラグ
    private bool includeBasePosition;

    // オブジェクトの名前を変更するかどうかのフラグ
    private bool renameInstance;

    // 配置個数（各軸方向）
    private Vector3Int copyCount;

    // 各オブジェクト間の間隔
    private Vector3 offsetPerAxis;

    // オブジェクトの配置基準位置
    private Vector3 originPosition;

    // エディタウィンドウのGUI描画処理
    private void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            // 対象オブジェクトのフィールド
            targetPrefab = EditorGUILayout.ObjectField("プレハブ", targetPrefab, typeof(GameObject), true) as GameObject;
           
            // 配置基準点にオブジェクトを配置するかどうか
            includeBasePosition = EditorGUILayout.Toggle("基準位置にも配置", includeBasePosition);

            // 各軸方向の配置間隔
            offsetPerAxis = EditorGUILayout.Vector3Field("配置間隔", offsetPerAxis);

            // 配置の基準位置
            originPosition = EditorGUILayout.Vector3Field("基準位置", originPosition);

            // 各軸方向の配置個数
            copyCount = EditorGUILayout.Vector3IntField("コピー数", copyCount);

            // 負の数を許可しない
            for (int i = 0; i < 3; i++)
            {
                if (copyCount[i] < 0)
                    copyCount[i] = 0;
            }

            // 生成したオブジェクトの名前を変更するかどうか
            renameInstance = EditorGUILayout.Toggle("オブジェクトの名前を変更", renameInstance);

            // 「生成」ボタンを表示し、押されたら配置処理を実行
            if (GUILayout.Button("生成"))
            {
                if (targetPrefab != null)
                {
                    GenerateCopies(includeBasePosition, copyCount, offsetPerAxis, originPosition, targetPrefab);
                }
            }
        }
    }

    // オブジェクトを配置する処理
    private void GenerateCopies(bool includeOrigin, Vector3Int countPerAxis, Vector3 spacingPerAxis, Vector3 startPosition, GameObject prefabToInstantiate)
    {
        // 配置数がゼロなら何もしない
        if (countPerAxis == Vector3Int.zero) return;

        // 生成する位置を保持するリスト
        List<Vector3> spawnPositions = new List<Vector3>();

        // 三重ループで全方向に配置位置を計算
        for (int x = countPerAxis.x; x >= 0; x--)
        {

            if (IsInitialPositionSkip(x, countPerAxis.x))
                continue;

            for (int y = countPerAxis.y; y >= 0; y--)
            {

                if (IsInitialPositionSkip(y, countPerAxis.y))
                    continue;

                for (int z = countPerAxis.z; z >= 0; z--)
                {

                    if (IsInitialPositionSkip(z, countPerAxis.z))
                        continue;

                    // 配置座標を計算
                    Vector3 spawnPos = new Vector3(
                        startPosition.x + spacingPerAxis.x * x,
                        startPosition.y + spacingPerAxis.y * y,
                        startPosition.z + spacingPerAxis.z * z);

                    // 重複を避けるため、既に追加された座標でなければリストに追加
                    if (!spawnPositions.Contains(spawnPos))
                        spawnPositions.Add(spawnPos);
                }
            }
        }

        // 配置基準点にオブジェクトを配置しない設定の場合、リストから削除
        if (!includeOrigin) spawnPositions.Remove(startPosition);

        // オブジェクトを生成
        CreateObjects(prefabToInstantiate, spawnPositions);

        Debug.Log(spawnPositions.Count + " 個生成しました。");
    }

    // 各軸の最大値（limit）に達したときに最初のループをスキップするための関数
    private bool IsInitialPositionSkip(int index, int max)
    {
        return index == max && index > 0;
    }

    // 実際にGameObjectを指定位置に複製生成する処理
    private void CreateObjects(GameObject prefabToInstantiate, List<Vector3> spawnPositions)
    {
        foreach (Vector3 position in spawnPositions)
        {
            // プレハブを各座標にエディタ上に生成（リンク維持）
            
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToInstantiate);

            // プレハブに名前を変更
            if (renameInstance)
            {
                instance.name += "_x" + position.x.ToString() +
                                 "_y" + position.y.ToString() +
                                 "_z" + position.z.ToString();
            }
                
            Undo.RegisterCreatedObjectUndo(instance, "Create Prefab Instance");
            instance.transform.position = position;

            // インスタンス生成の場合は下
            //Instantiate(prefabToInstantiate, position, Quaternion.identity);
        }
    }
}
