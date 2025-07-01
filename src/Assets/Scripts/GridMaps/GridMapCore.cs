using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// グリッドベースのマップシステムを管理するクラス
/// </summary>
public class GridMap
{
    GridCell[,] gridCell;

    Vector2Int mapSize;

    /// <summary>
    /// GridMapコンストラクタ - 指定サイズでグリッドを初期化
    /// </summary>
    /// <param name="vector2Int">マップサイズ</param>
    public GridMap(Vector2Int vector2Int)
    {
        this.mapSize = vector2Int;
        InitializeGrid();
    }

    /// <summary>
    /// 指定座標がマップの範囲内かどうかを判定
    /// </summary>
    /// <param name="pos">チェックする座標</param>
    /// <returns>範囲内の場合true</returns>
    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < mapSize.x && pos.y < mapSize.y;
    }

    /// <summary>
    /// 指定座標のGridCellを取得
    /// </summary>
    /// <param name="pos">取得する座標</param>
    /// <returns>GridCellオブジェクト（範囲外の場合はNULLTYPE）</returns>
    public GridCell GetGridCell(Vector2Int pos)
    {
        if (IsInBounds(pos))
            return gridCell[pos.x, pos.y];
        else
        {
            Debug.LogError("範囲外");
            // ヌル用コンストラクタで初期化したもので返す
            return new GridCell(BuildType.NULLTYPE);
        }
    }

    /// <summary>
    /// 指定座標にGridCellを設定
    /// </summary>
    /// <param name="cell">設定するGridCell</param>
    public void SetGridCell(GridCell cell)
    {
        if (IsInBounds(cell.GridPos))
            gridCell[cell.GridPos.x, cell.GridPos.y] = cell;
        else
        {
            Debug.LogError("範囲外");
        }
    }

    // <summary>
    /// 指定座標のGridCellを空の状態にリセット
    /// </summary>
    /// <param name="pos">リセットする座標</param>
    public void SetEmptyGridCell(Vector2Int pos)
    {
        if (IsInBounds(pos))
            gridCell[pos.x, pos.y].EmptyCell();
        else
        {
            Debug.LogError("範囲外");
        }
    }

    /// <summary>
    /// TilemapからタイルタイプをGridCellに設定
    /// </summary>
    /// <param name="tilemap">参照するTilemap</param>
    public void SetTileType(Tilemap tilemap)
    {
        if(gridCell == null)
        {
            Debug.LogError("未初期化");
        }

        // マップ全体をループしてタイルタイプを設定
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int vector3Int =  new Vector3Int()
                {
                    x = x,
                    y = y,
                    z = 0
                };
                // タイルが存在する場合、タイプを取得してセル に設定
                if (tilemap.HasTile(vector3Int))
                {
                    var tile = tilemap.GetTile(vector3Int) as CustomTile;
                    //Debug.Log($"Tile at {vector3Int}: {tile}");

                    if (tile != null)
                        gridCell[x, y].SetTileType(tile.tileType);
                }
            }
        }
    }

    /// <summary>
    /// グリッド全体を初期化
    /// </summary>
    void InitializeGrid()
    {
        // 全てを初期化
        gridCell = new GridCell[mapSize.x, mapSize.y];

        // ループ内でVector2Intのnewの宣言したくないので最初に一回
        Vector2Int forIndex =new Vector2Int(0,0);

        // 全座標にGridCellオブジェクトを生成
        for (; forIndex.x < mapSize.x; forIndex.x++)
        {
            forIndex.y = 0;
            for (; forIndex.y < mapSize.y; forIndex.y++)
            {  
                gridCell[forIndex.x, forIndex.y] = new GridCell(forIndex);
                //Debug.Log(gridCell[forIndex.x, forIndex.y]);
            }
        }
    }
}
/// <summary>
/// 地形タイルの種類を定義
/// </summary>
public enum TileType
{
    None,
    Tree,
    Rock,
}

/// <summary>
/// セル種類を定義するenum
/// </summary>
public enum BuildType
{
    None,      // 無し
    BaseCamp,  // 拠点本体
    Belt,      // ベルトコンベア
    Production,// 生産拠点
    Processing,// 加工施設
    Turret,    // 攻撃施設
    Wall,　　  // 防御施設
    NULLTYPE,  //エラー処理用
}

/// <summary>
/// グリッドの各セルを表すクラス
/// 位置、建物、タイル情報を管理
/// </summary>
public class GridCell
{
    public Vector2Int GridPos;
    public BuildType GridCellType;
    public GameObject GridObject;

    GridBuilding building;
    TileType tileType;

    // 最初にすべて初期化するコンストラクタ
    public GridCell(Vector2Int vector2Int)
    {
        GridPos = vector2Int;
        tileType = TileType.None;
        GridCellType = BuildType.None;
        GridObject = null;      
        building = null;
    }

    // エラー参照用コンストラクタ
    public GridCell(BuildType NULLTYPE)
    {
        GridPos = new(-1, -1);
        tileType = TileType.None;
        GridCellType = NULLTYPE;
        GridObject = null;
        building = null;

        Debug.LogError("何かしら良くないので要デバッグ");
    }

    /// <summary>
    /// 施設建設用コンストラクタ
    /// </summary>
    //　施設の建設に使うコンストラクタ
    public GridCell(Vector2Int vector2Int, BuildType cellType, GameObject gameObject, GridBuilding gridBuilding)
    {
        GridCellType = cellType;
        GridObject = gameObject;
        GridPos = vector2Int;
        building = gridBuilding;
    }

    /// <summary>
    /// 建物のロジッククラスを取得
    /// </summary>
    /// <returns>建物のクラス(派生クラス込み)</returns>
    public GridBuilding GetBuilding() => building;

    /// <summary>
    /// セルを空の状態にリセット
    /// </summary>
    public void EmptyCell()
    {
        GridCellType = BuildType.None;
        GridObject = null;
        building = null;
    }

    // <summary>
    /// タイルタイプを設定
    /// </summary>
    /// <param name="tileType">設定するタイルタイプ</param>
    public void SetTileType (TileType tileType) => this.tileType = tileType;

    /// <summary>
    /// セルが空かどうかを判定
    /// </summary>
    /// <returns>空の場合true</returns>
    public bool IsNoneCelltype() => GridCellType == BuildType.None;

    /// <summary>
    /// 指定したタイルタイプと同じかどうかを判定
    /// </summary>
    /// <param name="tiletype">比較するタイルタイプ</param>
    /// <returns>同じ場合true</returns>
    public bool SameTileType(TileType tiletype)
    {
        return this.tileType == tiletype;
    }

    /// <summary>
    /// 建物のサイズを取得
    /// </summary>
    /// <returns>建物サイズ（建物がない場合は1x1x0）</returns>
    public Vector3 GetBuildingSize()
    {
        if (building == null)
            return Vector3.one;

        return new()
        {
            x = building.MinBuildingPos.x,
            y = building.MaxBuildingPos.y,
            z = 0
        };
    }

    /// <summary>
    /// グリッドオブジェクトのスケールを取得
    /// </summary>
    /// <returns>オブジェクトスケール（オブジェクトがない場合は1x1x1）</returns>      
    public Vector3 GetGridObjectScale()
    {
        if(GridObject == null)
            return Vector3.one;

        return GridObject.transform.localScale;
    }  
}

/// <summary>
/// マップ上の建設可能な施設の設定を管理する構造体
/// </summary>
[System.Serializable]
public struct MapContent
{
    [SerializeField] Vector2Int minGridPos;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] BuildType gridCellType;
    [SerializeField] TileType canCreateTileType;
    [SerializeField] GameObject gridObject;
    [SerializeField] List<Transform> importTransforms;
    [SerializeField] List<Transform> exportTransforms;

    /// <summary>
    /// [HideInInspector]を設定して
    /// フィールド非表示時も値を保持し続けるようにする
    /// </summary>
    [HideInInspector]
    [SerializeField]
    ItemInformation item;

    // プロパティ(Editor拡張を使うため)
    public ItemInformation Iteminfo
    {
        get => item;
        set => item = value;
    }

    /// <summary>
    /// 最小グリッド座標を設定
    /// </summary>
    /// <param name="pos">設定する座標</param>
    public void SetMinGridPos(Vector2Int pos) => this.minGridPos = pos;

    // Public getter
    public Vector2Int MinGridPos => minGridPos;
    public Vector2Int GridSize => gridSize;
    public BuildType GridCellType => gridCellType;

    /// <summary>
    /// 建設可能なタイルタイプを取得
    /// </summary>
    /// <returns>タイルタイプ</returns>
    public TileType CanCreateTileType() => canCreateTileType;

    /// <summary>
    /// グリッドオブジェクトを取得
    /// </summary>
    /// <returns>GameObjectインスタンス</returns>
    public GameObject GridObject() => gridObject;

    /// <summary>
    /// 最大グリッド座標を計算
    /// </summary>
    /// <returns>最大座標</returns>
    public Vector2Int MaxGridPos() => minGridPos + GridSize - Vector2Int.one;

    /// <summary>
    /// インポート座標のハッシュセットを取得
    /// </summary>
    /// <returns>インポート座標のHashSet</returns>
    public readonly HashSet<Vector2Int> IｍportGridPos() => ConvertVector2Int(importTransforms);

    /// <summary>
    /// エクスポート座標のハッシュセットを取得
    /// </summary>
    /// <returns>エクスポート座標のHashSet</returns>
    public readonly HashSet<Vector2Int> ExportGridPos() => ConvertVector2Int(exportTransforms);

    /// <summary>
    /// TransformリストをVector2IntのHashSetに変換
    /// </summary>
    /// <param name="transforms">変換元のTransformリスト</param>
    /// <returns>変換されたVector2IntのHashSet</returns>
    readonly HashSet<Vector2Int> ConvertVector2Int(List<Transform> transforms)
    {
        HashSet<Vector2Int> vec2Int = new HashSet<Vector2Int>();

        foreach (Transform t in transforms)
        {
            // Transform座標を四捨五入してVector2Intに変換
            Vector3 pos = t.position;
            Vector2Int rounded = new Vector2Int
            {
                x = Mathf.RoundToInt(pos.x),
                y = Mathf.RoundToInt(pos.y) 
            };

            vec2Int.Add(rounded);
        }

        return vec2Int;
    } 
}
