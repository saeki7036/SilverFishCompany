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
        Vector3Int mapPos;
        if (IsInGridMap(mouseWorldDownPos))
        {
            // インデックスからの取得のため -1 をしている
            Vector2Int maxMapIndex = maxMapSize - Vector2Int.one;
            mapPos = new()
            {
                x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.x), ClampMin, maxMapIndex.x),
                y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.y), ClampMin, maxMapIndex.y),
                z = 0,
            };
        }
        else
        {
            mapPos = OutRangePos;
        }

        targetTransform.position = mapPos;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
       
    }
}
