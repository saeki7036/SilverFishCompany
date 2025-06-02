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
            Debug.LogError("�͈͊O");
            // �������ŕԂ�
            return new();
        }
    }

    public void SetGridCell(GridCell cell)
    {
        if (IsInBounds(cell.GridPos))
            gridCell[cell.GridPos.x, cell.GridPos.y] = cell;
        else
        {
            Debug.LogError("�͈͊O");
        }
    }


    // �v���p�e�B
    public GridCell[,] GridCells
    {
        get => gridCell; 
        set => gridCell = value;
    }


    void InitializeGrid()
    {
        // �S�Ă�������
        gridCell = new GridCell[mapSize.x, mapSize.y];

        // ���[�v����Vector2Int��new�̐錾�������Ȃ��̂ōŏ��Ɉ��
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

// 3�����_���W�����̃Z����ނ��`����enum
public enum CellType
{
    None,      // ����
    BaseCamp,  // ���_�{��
    Belt,      // �x���g�R���x�A
    Production,// ���Y���_
    Room,      // ����
    Corridor   // �ʘH
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
