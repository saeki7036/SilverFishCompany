using UnityEngine;

public class GridContent : MonoBehaviour
{
    [SerializeField]
    GridCell cell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridSetting();
    }

    void GridSetting()
    {
        GridMapManager.Instance.SetCell(cell);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
