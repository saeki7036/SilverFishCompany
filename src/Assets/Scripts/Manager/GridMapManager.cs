using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GridMapManager : MonoBehaviour
{
    [SerializeField]
    public Vector2Int mapSize; // グリッドサイズ（例: 60×60）

    [SerializeField]
    float gridAdjustScale = 0.5f;

    private GridMap gridMap;
   
    public GridCell GetCell(Vector2Int pos) => gridMap.GetGridCell(pos);

    public void SetCell(GridCell cell) => gridMap.SetGridCell(cell);
       
    public Vector2Int MaxMapSize => mapSize;

    public float GridAdjustScale() => gridAdjustScale;

    private static GridMapManager instance;
    public static GridMapManager Instance => instance;

    public void StartSetContent(MapContent content)
    {
        if(!IsInBounds(content.minGridPos) ||! IsInBounds(content.maxGridPos))
        {
            Debug.LogError(content + "範囲外です。");
            return;
        }

        if (content.GridCellType == CellType.None)
        {
            Debug.LogError(content + "CellTypeを設定してください。");
            return;
        }

        List<Vector2Int> SetCellList = new List<Vector2Int>();

        for(int x = content.minGridPos.x; x <= content.maxGridPos.x; x++)
        {
            for (int y = content.minGridPos.y; y <= content.maxGridPos.y; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);

                if (GetCell(currentPos).GridCellType != CellType.None)
                {
                    Debug.LogError("Cellが重複しています=>" + GetCell(currentPos).GridCellType);
                    return;
                }

                SetCellList.Add(currentPos);
            }
        }

        foreach(Vector2Int cell in SetCellList)
        {
            GridCell settingCell = new GridCell(cell, content.GridCellType, content.GridObject);
            SetCell(settingCell);
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        instance = this;

        InitializeGrid();
    }


    public void BeltSetting(List<Vector3Int> vector3Ints, List<GameObject> objects)
    {
        if(vector3Ints.Count != objects.Count)
        {
            Debug.LogError("不正な値");
            return;
        }
        List < GridCell > BeltCellList = new List < GridCell >();

        for (int i = 0; i < vector3Ints.Count; i++)
        {
            Vector2Int vector2Int = new(vector3Ints[i].x, vector3Ints[i].y);
            
            GridCell cell = new GridCell(vector2Int, CellType.Belt, objects[i]);

            SetCell(cell);

            BeltCellList.Add(cell);
        }
    }

    private void InitializeGrid()
    { 
        gridMap = new GridMap(mapSize);

        //gridMap = new GameObject[mapSize.x, mapSize.y];      
    }

    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < mapSize.x && pos.y < mapSize.y;
    }
}
