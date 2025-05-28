using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GridMapManager : MonoBehaviour
{
    [SerializeField]
    public Vector2Int mapSize; // グリッドサイズ（例: 20×20）

    public GameObject[,] gridMap; 

    public GameObject GetCell(int x,int y) => gridMap[x,y];

    public GameObject SetCell(int x, int y, GameObject obj) => gridMap[x, y] = obj;

    private static GridMapManager instance;
    public static GridMapManager Instance => instance;

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
    void PlaceObject(int x, int y, GameObject prefab)
    {
        if (gridMap[x, y] == null)
        {
            GameObject obj = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
            gridMap[x, y] = obj;
        }
    }
    private void InitializeGrid()
    {
        gridMap = new GameObject[mapSize.x, mapSize.y];

        //gridMap = new GameObject[mapSize.x, mapSize.y];      
    }

    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < mapSize.x && pos.y < mapSize.y;
    }
}
