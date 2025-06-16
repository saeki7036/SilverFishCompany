using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class GridMap
{
    GridCell[,] gridCell;

    Vector2Int mapSize;

    public GridMap(Vector2Int vector2Int)
    {
        this.mapSize = vector2Int;
        InitializeGrid();
    }

    public GridMap (int width, int height)
    {       
        this.mapSize.x = width;
        this.mapSize.y = height;
        InitializeGrid();
    }

    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < mapSize.x && pos.y < mapSize.y;
    }

    public GridCell GetGridCell(Vector2Int pos)
    {
        if (IsInBounds(pos))
            return gridCell[pos.x, pos.y];
        else
        {
            Debug.LogError("範囲外");
            // ヌル用コンストラクタで初期化したもので返す
            return new GridCell(CellType.NULLTYPE);
        }
    }

    public void SetGridCell(GridCell cell)
    {
        if (IsInBounds(cell.GridPos))
            gridCell[cell.GridPos.x, cell.GridPos.y] = cell;
        else
        {
            Debug.LogError("範囲外");
        }
    }

    // プロパティ
    public GridCell[,] GridCells
    {
        get => gridCell; 
        set => gridCell = value;
    }

    void InitializeGrid()
    {
        // 全てを初期化
        gridCell = new GridCell[mapSize.x, mapSize.y];

        // ループ内でVector2Intのnewの宣言したくないので最初に一回
        Vector2Int forIndex =new(0,0);
        
        for (; forIndex.x < mapSize.x; forIndex.x++)
        {
            for (; forIndex.y < mapSize.y; forIndex.y++)
            {  
                gridCell[forIndex.x, forIndex.y] = new GridCell(forIndex);
            }
        }
    }
}

// セル種類を定義するenum
public enum CellType
{
    None,      // 無し
    BaseCamp,  // 拠点本体
    Belt,      // ベルトコンベア
    Production,// 生産拠点
    processing,// 加工施設
    Room,      // 部屋
    Corridor,  // 通路
    NULLTYPE,  //エラー処理用
}

[System.Serializable]
public struct GridCell
{
    public Vector2Int GridPos;
    public CellType GridCellType;
    public GameObject GridObject;
    public GridBuilding Building;

    // 最初にすべて初期化するコンストラクタ
    public GridCell(Vector2Int vector2Int)
    {
        GridPos = vector2Int;
        GridCellType = CellType.None;
        GridObject = null;      
        Building = null;
    }

    // エラー参照用コンストラクタ
    public GridCell(CellType NULLTYPE)
    {
        GridPos = new(-1, -1);
        GridCellType = NULLTYPE;
        GridObject = null;
        Building = null;

        Debug.LogError("何かしら良くないので要デバッグ");
    }

    //　施設の建設に使うコンストラクタ
    public GridCell(Vector2Int vector2Int, CellType cellType, GameObject gameObject, GridBuilding gridBuilding)
    {
        GridCellType = cellType;
        GridObject = gameObject;
        GridPos = vector2Int;
        Building = gridBuilding;
    }

    public bool IsNoneCelltype() => GridCellType == CellType.None;

    public Vector3 GetBuildingSize()
    {
        if (Building == null)
            return Vector3.one;

        return new()
        {
            x = Building.MinBuildingPos.x,
            y = Building.MaxBuildingPos.y,
            z = 0
        };
    }
            
    public Vector3 GetGridObjectScale()
    {
        if(GridObject == null)
            return Vector3.one;

        return GridObject.transform.localScale;
    }  
}

[System.Serializable]
public struct MapContent
{
    [SerializeField] Vector2Int minGridPos;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] CellType gridCellType;
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

    // Public getter
    public Vector2Int MinGridPos => minGridPos;
    public Vector2Int GridSize => gridSize;
    public CellType GridCellType => gridCellType;
    public GameObject GridObject => gridObject;
    
    public  Vector2Int maxGridPos => minGridPos + GridSize - Vector2Int.one;

    public readonly HashSet<Vector2Int> IｍportGridPos() => ConvertVector2Int(importTransforms);
    public readonly HashSet<Vector2Int> ExportGridPos() => ConvertVector2Int(exportTransforms);

    readonly HashSet<Vector2Int> ConvertVector2Int(List<Transform> transforms)
    {
        HashSet<Vector2Int> vec2Int = new HashSet<Vector2Int>();

        foreach (Transform t in transforms)
        {
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
