using UnityEngine;

public class TargetCursol : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;

    [SerializeField]
    Vector2Int maxMapSize = new(59, 59);

    const int ClampMin = 0;

    public void InputRegister(MouseController input)
    {
        input.LeftDownEvent += SetTargetTransform;
    }

    void SetTargetTransform(Vector3 mouseWorldDownPos)
    {
        Vector3Int mapPos = new()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.x), ClampMin, maxMapSize.x),
            y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.y), ClampMin, maxMapSize.y),
            z = 0,
        };

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
