using UnityEngine;

public class GridContent : MonoBehaviour
{
    [SerializeField]
    MapContent content;

    public bool IsBelt() => content.GridCellType == BuildType.Belt;

    // getter
    public MapContent Content 
    {
        get => content;
    }

    Vector2Int CrampGridPos()
    {
        Vector2Int GridIndexSIze = GridMapManager.Instance.MaxMapSize - Vector2Int.one;

        return new Vector2Int()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(transform.position.x), 0, GridIndexSIze.x),
            y = Mathf.Clamp(Mathf.RoundToInt(transform.position.y), 0, GridIndexSIze.y),
        };
    } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2Int GridPos = CrampGridPos();
       
        content.SetMinGridPos(GridPos);
        
        GridSetting();

        // Debug.Log(Content.GridSize.ToString() + ":" + Content.Iteminfo.ItemCategory);
    }

    void GridSetting()
    {
        GridMapManager.Instance.StartSetContent(content);
    }
}
