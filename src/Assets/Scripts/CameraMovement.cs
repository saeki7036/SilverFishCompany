using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    float CameraBaseMoveSpeed = 3f;

    [SerializeField]
    Vector2 ThresholdValue = new(5f, 3f);

    
    Vector2Int MaxMapSize => GridMapManager.Instance.MaxMapSize;

    const float ClampMin = 0f;
    

    public void InputRegister(MouseController input)
    {
        input.RightClickEvent += CameraMove;
    }

    int GetMoveDirectionAxis(float thisPos,float mousePos,float thresholdValue)
    {
        //Debug.Log(thisPos - mousePos);
        //Mathf.RoundToInt(thisPos - mousePos)

        if (thisPos - mousePos > thresholdValue)
            return -1;
        
        if (thisPos - mousePos < -thresholdValue)
            return 1;

        return 0;
    }

    float GetMoveSpeedAxis(float thisPos, float mousePos)
    {
        return (Mathf.Abs(thisPos - mousePos) + CameraBaseMoveSpeed);
    }

    void CameraMove(Vector3 mouseWorldPos)
    {
        Vector2Int moveDirection = new()
        {
            x = GetMoveDirectionAxis(transform.position.x, mouseWorldPos.x,ThresholdValue.x),
            y = GetMoveDirectionAxis(transform.position.y, mouseWorldPos.y,ThresholdValue.y),
        };
        Debug.Log(moveDirection);

        Vector2 moveSpeed = new()
        {
            x = GetMoveSpeedAxis(transform.position.x, mouseWorldPos.x),
            y = GetMoveSpeedAxis(transform.position.y, mouseWorldPos.y),
        };

        Vector3 afterMovePos = new()
        {
            x = transform.position.x + (moveDirection.x * moveSpeed.x * Time.deltaTime),
            y = transform.position.y + (moveDirection.y * moveSpeed.y * Time.deltaTime),
            z = transform.position.z,
        };

        transform.position = afterMovePos;

        PositionClamp();
    }

    void PositionClamp()
    {
        Vector3 mapPos = new()
        {
            x = Mathf.Clamp(transform.position.x, ClampMin, MaxMapSize.x),
            y = Mathf.Clamp(transform.position.y, ClampMin, MaxMapSize.y),
            z = transform.position.z,
        };

        transform.position = mapPos;
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
