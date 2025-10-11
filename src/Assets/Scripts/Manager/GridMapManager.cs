using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridMapManager : MonoBehaviour
{
    // グリッドベースのマップ管理を行うマネージャークラス(シングルトン)

    [SerializeField]
    public Vector2Int mapSize; // マップの全体のグリッドサイズ（例: 60×60）

    [SerializeField]
    float gridAdjustScale = 0.5f;// グリッド調整用のスケール係数

    [SerializeField]
    Tilemap tilemap;// タイルマップコンポーネントの参照

    GridMap gridMap;// グリッドマップデータの管理クラス

    // 建物タイプ別の建物管理辞書
    // Key: 建物タイプ, Value: 該当建物のHashSet
    Dictionary<BuildType, HashSet<GridBuilding>> BuildingDictionary;

    static GridMapManager instance;// シングルトンインスタンス

    public static GridMapManager Instance => instance;// GridMapManagerのインスタンスのアクセサ

    /// <summary>
    /// グリッドセルを設定
    /// </summary>
    /// <param name="cell">設定するグリッドセル</param>
    void SetCell(GridCell cell) => gridMap.SetGridCell(cell);

    /// <summary>
    /// 指定位置のグリッドセルを取得
    /// </summary>
    /// <param name="pos">取得したいセルの座標</param>
    /// <returns>指定位置のグリッドセル</returns>
    public GridCell GetCell(Vector2Int pos) => gridMap.GetGridCell(pos);

    /// <summary>
    /// マップの最大サイズを取得
    /// </summary>
    public Vector2Int MaxMapSize => mapSize;

    /// <summary>
    /// グリッド調整用のスケール値を取得
    /// </summary>
    public float GridAdjustScale() => gridAdjustScale;

    /// <summary>
    /// 建物情報のDictionaryを取得(getter)
    /// </summary>
    /// <returns>newで初期化されたDictionaryを取得</returns>
    public Dictionary<BuildType, HashSet<GridBuilding>> GetDictionary() => new(BuildingDictionary);

    /// <summary>
    /// 指定座標がマップ範囲内かどうかを判定
    /// </summary>
    /// <param name="pos">判定する座標</param>
    /// <returns>true: 範囲内, false: 範囲外</returns>
    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < mapSize.x && pos.y < mapSize.y;
    }

    /// <summary>
    /// 指定タイプの建物を一括操作
    /// BuildType.Beltの場合はBaseCampのImportItem()を実行
    /// その他の場合は該当建物のOperat()を実行
    /// </summary>
    /// <param name="cellType">操作する建物のタイプ</param>
    public void OperatBuilding(BuildType cellType)
    {
        // CellType.Beltのみ別処理
        if (cellType == BuildType.Belt)
        {
            if(BuildingDictionary[BuildType.BaseCamp] == null)
            {
                Debug.Log("Beltタスクが実行されたがBaseCampが初期化されてない");
                return;
            }

            // CellType.BaseCampを実行
            foreach (GridBuilding basecamp in BuildingDictionary[BuildType.BaseCamp])
            {
                basecamp.ImportItem();
            }
            return;
        }
        // 例外処理
        else if (!BuildingDictionary.ContainsKey(cellType))
        {
            //Debug.Log("扱わないTypeです:" + cellType);
            return;
        }
        else 
        {
            // CellTypeを実行
            foreach (GridBuilding building in BuildingDictionary[cellType])
            {
                building?.Operat();
            }
            return;
        } 
    }

    /// <summary>
    /// マップコンテンツの配置処理
    /// 範囲チェック、重複チェックを行い建物を生成・配置
    /// </summary>
    /// <param name="content">配置するマップコンテンツ</param>
    public void StartSetContent(MapContent content)
    {
        // 範囲チェック
        if (!IsInBounds(content.MinGridPos) ||! IsInBounds(content.MaxGridPos()))
        {
            Debug.LogError(content + "範囲外です。");
            return;
        }

        // BuildTypeの有効性チェック
        if (content.GridCellType == BuildType.None)
        {
            Debug.LogError(content + "CellTypeを設定してください。");
            return;
        }

        List<Vector2Int> SetCellList = new List<Vector2Int>();

        // 配置範囲内の全セルをチェック
        for (int x = content.MinGridPos.x; x <= content.MaxGridPos().x; x++)
        {
            for (int y = content.MinGridPos.y; y <= content.MaxGridPos().y; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);

                // セルの重複チェック
                if (GetCell(currentPos).GridCellType != BuildType.None)
                {
                    Debug.LogError("Cellが重複しています=>" + GetCell(currentPos).GridCellType);
                    return;
                }

                SetCellList.Add(currentPos);
            }
        }

        // 建物インスタンスを生成
        GridBuilding gridBuilding = CreateBuildingFactory.CreateBuilding(
            content.GridCellType,
            content.MinGridPos,
            content.MaxGridPos(),
            content.IｍportGridPos(),
            content.ExportGridPos(),
            content.Iteminfo
            );

        //gridBuilding.ExportPos = content.ExportGridPos();// 排出位置
        //gridBuilding.ImportPos = content.IｍportGridPos();// 取り込み位置

        // 各セルに建物情報を設定
        foreach (Vector2Int cell in SetCellList)
        {
            GridCell settingCell = new GridCell(cell, content.GridCellType, content.GridObject(), gridBuilding);
            SetCell(settingCell);
        }

        // 建物を管理辞書に登録
        SetBuildingDictionary(content.GridCellType, gridBuilding);
    }

    void Awake()
    {
        // 初期化処理

        // シングルトンパターンの実装
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        instance = this;

        // グリッドマップの初期化
        gridMap = new GridMap(mapSize);
        BuildingDictionary = new Dictionary<BuildType, HashSet<GridBuilding>>();

        // タイルマップの設定
        gridMap.SetTileType(tilemap);
    }

    /// <summary>
    /// 建物を辞書に追加登録
    /// BuildType.BeltとBuildType.Noneは除外処理あり
    /// </summary>
    /// <param name="cellType">建物のタイプ</param>
    /// <param name="building">登録する建物</param>
    void SetBuildingDictionary(BuildType cellType, GridBuilding building)
    {
        // 例外処理
        if (cellType == BuildType.None || cellType == BuildType.NULLTYPE || building == null)
        {
            Debug.LogAssertion("nullかどうか要確認");
            return;
        }

        // DictionaryにないならHashSetを初期化
        if (!BuildingDictionary.ContainsKey(cellType))
        {
            BuildingDictionary[cellType] = new HashSet<GridBuilding>();
        }

        /*
        else if (cellType == BuildType.Belt)
        {
            Debug.Log("Belt除外");
            return;
        }
        */

        // Dictionaryに追加
        BuildingDictionary[cellType].Add(building);
    }

    /// <summary>
    /// Vector3IntをVector2Intに変換
    /// </summary>
    /// <param name="vector3Int">変換元のVector3Int</param>
    /// <returns>変換後のVector2Int</returns>
    Vector2Int Conversion2D(Vector3Int vector3Int) => new(vector3Int.x, vector3Int.y);

    /// <summary>
    /// ベルトコンベアの設定処理
    /// パス上の各セルにベルト建物を生成し、輸送経路を設定
    /// </summary>
    /// <param name="vector3Ints">ベルトのパス座標リスト</param>
    /// <param name="objects">各座標に対応するゲームオブジェクトリスト</param>
    /// <param name="startInportPos">開始地点の取り込み座標</param>
    /// <param name="endExportPos">終了地点の排出座標</param>
    public void BeltSetting(List<Vector3Int> vector3Ints, List<GameObject> objects, Vector3Int startInportPos , Vector3Int endExportPos)
    {
        // リストサイズの整合性チェック
        if (vector3Ints.Count != objects.Count)
        {
            Debug.LogError("不正な値");
            return;
        }

        // Vector3IntをVector2Intに変換
        List<Vector2Int> vector2Ints = new List<Vector2Int>();

        for (int i = 0; i < vector3Ints.Count; i++)
        {
            vector2Ints.Add(new(vector3Ints[i].x, vector3Ints[i].y));
        }

        List<GridCell> BeltCellList = new List<GridCell>();

        // 各ベルトセルの設定処理
        for (int i = 0; i < vector3Ints.Count; i++)
        {
            Vector2Int vector2Int = Conversion2D(vector3Ints[i]);

            // 取り込み位置の設定（最初のセルは開始地点、それ以外は前のセル）
            HashSet<Vector2Int> inportPos = new HashSet<Vector2Int>()
            {
               Conversion2D(i == 0 ? startInportPos : vector3Ints[i - 1])
         　 };

            // 排出位置の設定（最後のセルは終了地点、それ以外は次のセル）
            HashSet<Vector2Int> exportPos = new HashSet<Vector2Int>()
            {
                Conversion2D(i + 1 == vector3Ints.Count ? endExportPos : vector3Ints[i + 1])
            };

            //Debug.Log("this:" + vector2Int + "import:" + inportPos.First() + "export:" + exportPos.First()); 

            // ベルトの生成
            BeltBuilding beltBuilding = new BeltBuilding(
                vector2Int,
                vector2Int,
                inportPos, 
                exportPos);

            // グリッドセルを生成・設定
            GridCell cell = new GridCell(vector2Int, BuildType.Belt, objects[i], beltBuilding);

            SetCell(cell);

            BeltCellList.Add(cell);

            SetBuildingDictionary(BuildType.Belt, beltBuilding);
        }
    }

    /// <summary>
    /// 指定位置のコンテンツを削除
    /// セルを空の状態に戻す
    /// </summary>
    /// <param name="point">削除する座標</param>
    public void DestroyContent(Vector2Int point)
    {
        // Celltype取得
        var DestroyCellType = GetCell(point).GridCellType;

        // Building取得
        var DestroyBuilding = GetCell(point).GetBuilding();

        // アイテム情報削除
        DestroyBuilding.DestoryItem();

        // Dictionaryから削除
        RemoveBuildingDictionary(DestroyCellType, DestroyBuilding);

        // GridMap内の位置を取得
        Vector2Int minPos = DestroyBuilding.MinBuildingPos, maxPos = DestroyBuilding.MaxBuildingPos;

        // 多重ループで削除
        for(int x = minPos.x; x <= maxPos.x; x++)
            for (int y = minPos.y; y <= maxPos.y; y++)
            {
                gridMap.SetEmptyGridCell(new(x,y));
            }

        DestroyBuilding = null;
    }

    /// <summary>
    /// 建物を辞書から削除
    /// BuildType.BaseCampとNullは除外処理あり
    /// </summary>
    /// <param name="cellType">削除する建物のタイプ</param>
    /// <param name="building">削除する建物</param>
    void RemoveBuildingDictionary(BuildType cellType, GridBuilding building)
    {
        if (cellType == BuildType.None || cellType == BuildType.NULLTYPE || building == null)
        {
            Debug.LogAssertion("nullかどうか要確認");
            return;
        }
        else if (cellType == BuildType.BaseCamp)
        {
            Debug.Log("BaseCamp除外");
            return;
        }
        else if (!BuildingDictionary.ContainsKey(cellType))
        {
            Debug.Log("ないCellTypeは除外");
            return;
        }

        Debug.Log(BuildingDictionary[cellType].Count);
        //
        BuildingDictionary[cellType].Remove(building);

        Debug.Log(BuildingDictionary[cellType].Count);
    }
}
