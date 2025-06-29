using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class MouseController : MonoBehaviour
{
    public event Action<Vector3> LeftDownEvent;
    public event Action<Vector3> RightDownEvent;

    public event Action<Vector3> LeftClickEvent;
    public event Action<Vector3> RightClickEvent;

    public event Action<Vector3> LeftUpEvent;
    public event Action<Vector3> RightUpEvent;

    [SerializeField]
    float MousePos_z = 0f;

    [SerializeField]
    float UIwidthMax = Screen.width;

    [SerializeField]
    float UIheightMax = Screen.height;

    const int ClampMin = 0;
    const int LeftInputNum = 0;
    const int RightInputNum = 1;

    struct MouseParameter
    {
        public Vector3 mouseUIPos;
        public Vector3 mouseUIDownPos;
        public Vector3 mouseWorldPos;
        public Vector3 mouseWorldDownPos;

        public void ResetMousePos()
        {
            mouseUIPos = Vector3.zero;
            mouseUIDownPos = Vector3.zero;
            mouseWorldPos = Vector3.zero;
            mouseWorldDownPos = Vector3.zero;     
        }
    }

    MouseParameter LeftParameter;
    MouseParameter RightParameter;

    void ClickDownInvoke(int num, Vector3 worldDownPos) 
    {
        if(num == LeftInputNum)
            LeftDownEvent?.Invoke(worldDownPos);
        if (num == RightInputNum)
            RightDownEvent?.Invoke(worldDownPos);
    }

    void ClickInvoke(int num, Vector3 worldPos)
    {
        if (num == LeftInputNum)
            LeftClickEvent?.Invoke(worldPos);
        if (num == RightInputNum)
            RightClickEvent?.Invoke(worldPos);
    }

    void ClickUpInvoke(int num, Vector3 worldUpPos)
    {
        if (num == LeftInputNum)
            LeftUpEvent?.Invoke(worldUpPos);
        if (num == RightInputNum)
            RightUpEvent?.Invoke(worldUpPos);
    }

    Vector3 GetWorldPoint(Vector3 UIPos)
    {
        Vector3 point = UIPos + Vector3.forward * MousePos_z;
        //Debug.Log(point + "+" + Camera.main.ScreenToWorldPoint(point));
        return Camera.main.ScreenToWorldPoint(point);
    }

    void MouseInputParameter(int num,ref MouseParameter parameter)
    {
        if (Input.GetMouseButtonDown(num))
        {
            parameter.mouseUIDownPos = GetTouchClamp();
            parameter.mouseWorldDownPos = GetWorldPoint(parameter.mouseUIDownPos);

            ClickDownInvoke(num,parameter.mouseWorldDownPos);
        }

        if (Input.GetMouseButton(num))
        {
            parameter.mouseUIPos = GetTouchClamp();
            parameter.mouseWorldPos = GetWorldPoint(parameter.mouseUIPos);

            ClickInvoke(num,parameter.mouseWorldPos);
        }
        
        if (Input.GetMouseButtonUp(num))
        {
            ClickUpInvoke(num, parameter.mouseWorldPos);

            parameter.ResetMousePos();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeftParameter = new MouseParameter();
        LeftParameter.ResetMousePos();

        RightParameter = new MouseParameter();
        RightParameter.ResetMousePos();

        //Debug.Log(Screen.width + "+" + Screen.height);
        //Debug.Log(UIwidthMax + "+" + UIheightMax);
    }


    // Update is called once per frame
    void Update()
    {
        MouseInputParameter(LeftInputNum,ref LeftParameter);

        MouseInputParameter(RightInputNum,ref RightParameter);
    }

    Vector3 GetTouchClamp()
    {
        Vector3 ClampPosition = Input.mousePosition;
       
        ClampPosition.y = Mathf.Clamp(ClampPosition.y, ClampMin, UIheightMax);
        ClampPosition.x = Mathf.Clamp(ClampPosition.x, ClampMin, UIwidthMax);

        //Debug.Log(Input.mousePosition + "+" + ClampPosition);
        return ClampPosition;
    }
}
