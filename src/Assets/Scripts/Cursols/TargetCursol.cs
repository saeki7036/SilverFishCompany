using UnityEngine;

public class TargetCursol : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;

    Vector2Int maxMapSize => GridMapManager.Instance.MaxMapSize;

    float GridAdjustScale => GridMapManager.Instance.GridAdjustScale();

    const int ClampMin = 0;

    //範囲外の場合にカメラ外に移動させるための数値
    static readonly Vector3Int OutRangePos = new(9999, 9999, 0);

    // インデックスからの取得のため -1 をしている
    Vector2Int MaxMapIndex => maxMapSize - Vector2Int.one;

    Vector3 StartCorsolScale;

    public void InputRegister(MouseController input)
    {
        input.LeftDownEvent += SetTargetTransform;
    }

    bool IsInGridMap(Vector3 mouseWorldDownPos)
    {
        if (mouseWorldDownPos.x < -GridAdjustScale ||
            maxMapSize.x - GridAdjustScale < mouseWorldDownPos.x)
            return false;

        if (mouseWorldDownPos.y < -GridAdjustScale ||
            maxMapSize.y - GridAdjustScale < mouseWorldDownPos.y)
            return false;

        return true;
    }

    void SetTargetTransform(Vector3 mouseWorldDownPos)
    {
        if (!IsInGridMap(mouseWorldDownPos))
        {
            targetTransform.position = OutRangePos;
            return;
        }

        Vector2Int mapPos2DInt = new()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.x), ClampMin, MaxMapIndex.x),
            y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.y), ClampMin, MaxMapIndex.y),
        };

        GameObject gridObject = GridMapManager.Instance.GetCell(mapPos2DInt).GridObject;

        if (gridObject == null)
        {
            targetTransform.position = new Vector3()
            {
                x = mapPos2DInt.x,
                y = mapPos2DInt.y,
                z = 0
            };

            targetTransform.localScale = StartCorsolScale;
            return;
        }

        Vector3 contentObjectWorldPosition = gridObject.transform.position;

        targetTransform.position = contentObjectWorldPosition;

        Vector3 contentObjectScale = gridObject.transform.localScale;
           
        targetTransform.localScale = new Vector3()
        {
            x = StartCorsolScale.x * contentObjectScale.x,
            y = StartCorsolScale.y * contentObjectScale.y,
            z = 1
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCorsolScale = transform.localScale;
    }
    
    // Update is called once per frame
    void Update()
    {
       
    }
}
