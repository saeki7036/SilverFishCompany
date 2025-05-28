using UnityEngine;

public class TargetCursol : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;

    [SerializeField]
    Vector2Int maxMapSize = new(59, 59);

    const int ClampMin = 0;

    //範囲外の場合にカメラ外に移動させるための数値
    static readonly Vector3Int OutRangePos = new(9999, 9999, 0);

    public void InputRegister(MouseController input)
    {
        input.LeftDownEvent += SetTargetTransform;
    }

    bool IsInGridMap(Vector3 mouseWorldDownPos)
    {
        if (mouseWorldDownPos.x < 0 || maxMapSize.x < mouseWorldDownPos.x)
            return false;

        if (mouseWorldDownPos.y < 0 || maxMapSize.y < mouseWorldDownPos.y)
            return false;

        return true;
    }

    void SetTargetTransform(Vector3 mouseWorldDownPos)
    {
        Vector3Int mapPos;
        if (IsInGridMap(mouseWorldDownPos))
        {
            mapPos = new()
            {
                x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.x), ClampMin, maxMapSize.x),
                y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.y), ClampMin, maxMapSize.y),
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
