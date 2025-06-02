using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
            // 初期化で返す
            return new();
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

// 3次元ダンジョンのセル種類を定義するenum
public enum CellType
{
    None,      // 無し
    BaseCamp,  // 拠点本体
    Belt,      // ベルトコンベア
    Production,// 生産拠点
    Room,      // 部屋
    Corridor   // 通路
}

[System.Serializable]
public struct GridCell
{
    public Vector2Int GridPos;
    public CellType GridCellType;
    public GameObject GridObject;

    public GridCell(Vector2Int vector2Int)
    {
        GridCellType = CellType.None;
        GridObject = null;
        GridPos = vector2Int;
    }

    public GridCell(GridCell gridCell)
    {
        GridCellType = gridCell.GridCellType;
        GridObject = gridCell.GridObject;
        GridPos = gridCell.GridPos;              
    }

    public GridCell(Vector2Int vector2Int, CellType cellType, GameObject gameObject)
    {
        GridCellType = cellType;
        GridObject = gameObject;
        GridPos = vector2Int;
    }
}
