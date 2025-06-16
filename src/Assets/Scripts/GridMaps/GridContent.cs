using UnityEngine;

public class GridContent : MonoBehaviour
{
    [SerializeField]
    MapContent content;

    // getter
    public MapContent Content 
    {
        get => content;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Debug.Log(Content.GridSize.ToString() + ":" + Content.Iteminfo.ItemCategory);
        GridSetting();
    }

    void GridSetting()
    {
        GridMapManager.Instance.StartSetContent(content);
    }
}
