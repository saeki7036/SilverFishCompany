using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class GridMapManager : MonoBehaviour
{
    [SerializeField]
    public Vector2Int mapSize; // グリッドサイズ（例: 60×60）

    [SerializeField]
    float gridAdjustScale = 0.5f;

    [SerializeField]
    Tilemap tilemap = null;

    GridMap gridMap;

    Dictionary<BuildType, HashSet<GridBuilding>> BuildingDictionary;

    static GridMapManager instance;
    public static GridMapManager Instance => instance;

    public GridCell GetCell(Vector2Int pos) => gridMap.GetGridCell(pos);

    public void SetCell(GridCell cell) => gridMap.SetGridCell(cell);

    public void SetTile(Tilemap tilemap) => gridMap.SetTileType(tilemap);


    public Vector2Int MaxMapSize => mapSize;

    public float GridAdjustScale() => gridAdjustScale;

    Vector2Int Conversion2D(Vector3Int vector3Int) => new(vector3Int.x, vector3Int.y);

    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < mapSize.x && pos.y < mapSize.y;
    }

    public void SetBuilding(BuildType cellType, GridBuilding building)
    {
        // 例外処理
        if(cellType == BuildType.None || building == null)
        {
            Debug.LogAssertion("nullかどうか要確認");
            return;
        }
        // DictionaryにないならHashSetを初期化
        if (!BuildingDictionary.ContainsKey(cellType))
        {
            BuildingDictionary[cellType] = new HashSet<GridBuilding>();
        }

        else if(cellType == BuildType.Belt)
        {
            Debug.Log("Belt除外");
            return;
        }

        BuildingDictionary[cellType].Add(building);
    }


    public void RemoveBuilding(BuildType cellType, GridBuilding building)
    {
        if (cellType == BuildType.None || building == null)
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
        
        //BuildingDictionary[cellType].Remove(building);
    }

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

            foreach (GridBuilding basecamp in BuildingDictionary[BuildType.BaseCamp])
            {
                basecamp.ImportItem();
            }
            return;
        }

        else if (!BuildingDictionary.ContainsKey(cellType))
        {
            Debug.Log("扱わないTypeです:" + cellType);
            return;
        }

        else 
        {
            foreach (GridBuilding building in BuildingDictionary[cellType])
            {
                building.Operat();
            }
            return;
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

        SetTile(tilemap);
    }

    void InitializeGrid()
    {
        gridMap = new GridMap(mapSize);
        BuildingDictionary = new Dictionary<BuildType, HashSet<GridBuilding>>();
    }


    public void StartSetContent(MapContent content)
    {
        if(!IsInBounds(content.MinGridPos) ||! IsInBounds(content.MaxGridPos()))
        {
            Debug.LogError(content + "範囲外です。");
            return;
        }

        if (content.GridCellType == BuildType.None)
        {
            Debug.LogError(content + "CellTypeを設定してください。");
            return;
        }

        List<Vector2Int> SetCellList = new List<Vector2Int>();

        for(int x = content.MinGridPos.x; x <= content.MaxGridPos().x; x++)
        {
            for (int y = content.MinGridPos.y; y <= content.MaxGridPos().y; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);

                if (GetCell(currentPos).GridCellType != BuildType.None)
                {
                    Debug.LogError("Cellが重複しています=>" + GetCell(currentPos).GridCellType);
                    return;
                }

                SetCellList.Add(currentPos);
            }
        }

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

        foreach (Vector2Int cell in SetCellList)
        {
            GridCell settingCell = new GridCell(cell, content.GridCellType, content.GridObject(), gridBuilding);
            SetCell(settingCell);
        }

        SetBuilding(content.GridCellType, gridBuilding);
    }

    
    public void BeltSetting(List<Vector3Int> vector3Ints, List<GameObject> objects, Vector3Int startInportPos , Vector3Int endExportPos)
    {
        if(vector3Ints.Count != objects.Count)
        {
            Debug.LogError("不正な値");
            return;
        }

        List<Vector2Int> vector2Ints = new List<Vector2Int>();

        for (int i = 0; i < vector3Ints.Count; i++)
        {
            vector2Ints.Add(new(vector3Ints[i].x, vector3Ints[i].y));
        }

        List<GridCell> BeltCellList = new List<GridCell>();

        for (int i = 0; i < vector3Ints.Count; i++)
        {
            Vector2Int vector2Int = Conversion2D(vector3Ints[i]);

            HashSet<Vector2Int> inportPos = new HashSet<Vector2Int>()
            {
               Conversion2D(i == 0 ? startInportPos : vector3Ints[i - 1])
         　 };
            HashSet<Vector2Int> exportPos = new HashSet<Vector2Int>()
            {
                Conversion2D(i + 1 == vector3Ints.Count ? endExportPos : vector3Ints[i + 1])
            };

            //Debug.Log("this:" + vector2Int + "import:" + inportPos.First() + "export:" + exportPos.First()); 

            BeltBuilding beltBuilding = new BeltBuilding(
                vector2Int,
                vector2Int,
                inportPos, 
                exportPos);

            GridCell cell = new GridCell(vector2Int, BuildType.Belt, objects[i], beltBuilding);

            SetCell(cell);

            BeltCellList.Add(cell);
        }
    }

    public void DestroyContent(Vector2Int point)
    {
        gridMap.SetEmptyGridCell(point);
    }
}
