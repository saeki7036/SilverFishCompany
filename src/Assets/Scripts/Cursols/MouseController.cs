using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class MouseController : MonoBehaviour
{
    // 各種マウスイベント（ワールド座標で通知）
    public event Action<Vector3> LeftDownEvent;
    public event Action<Vector3> RightDownEvent;

    public event Action<Vector3> LeftClickEvent;
    public event Action<Vector3> RightClickEvent;

    public event Action<Vector3> LeftUpEvent;
    public event Action<Vector3> RightUpEvent;

    [SerializeField]
    float MousePos_z = 0f;// Z座標補正値

    [SerializeField]
    float UIwidthMax = Screen.width;// UI上のX座標の最大（スクリーン幅）

    [SerializeField]
    float UIheightMax = Screen.height;// UI上のY座標の最大（スクリーン高さ）

    const int ClampMin = 0; // 画面外判定の最小値（0以下を許可しない）
    const int LeftInputNum = 0;  // 左クリックに相当するマウスボタン番号
    const int RightInputNum = 1; // 右クリックに相当するマウスボタン番号

    struct MouseParameter
    {
        public Vector3 mouseUIPos;         // 現在のUI座標
        public Vector3 mouseUIDownPos;     // 押下時のUI座標
        public Vector3 mouseWorldPos;      // 現在のワールド座標
        public Vector3 mouseWorldDownPos;  // 押下時のワールド座標

        public void ResetMousePos()
        {
            mouseUIPos = Vector3.zero;
            mouseUIDownPos = Vector3.zero;
            mouseWorldPos = Vector3.zero;
            mouseWorldDownPos = Vector3.zero;     
        }
    }

    MouseParameter LeftParameter;  // 左ボタン用パラメータ
    MouseParameter RightParameter; // 右ボタン用パラメータ

    /// <summary>
    /// マウスの押下時のイベント呼び出し
    /// </summary>
    void ClickDownInvoke(int num, Vector3 worldDownPos) 
    {
        if(num == LeftInputNum)
            LeftDownEvent?.Invoke(worldDownPos);
        if (num == RightInputNum)
            RightDownEvent?.Invoke(worldDownPos);
    }

    /// <summary>
    /// マウスボタンが押されている間のイベント呼び出し
    /// </summary>
    void ClickInvoke(int num, Vector3 worldPos)
    {
        if (num == LeftInputNum)
            LeftClickEvent?.Invoke(worldPos);
        if (num == RightInputNum)
            RightClickEvent?.Invoke(worldPos);
    }

    /// <summary>
    /// マウスボタンが離されたときのイベント呼び出し
    /// </summary>
    void ClickUpInvoke(int num, Vector3 worldUpPos)
    {
        if (num == LeftInputNum)
            LeftUpEvent?.Invoke(worldUpPos);
        if (num == RightInputNum)
            RightUpEvent?.Invoke(worldUpPos);
    }

    /// <summary>
    /// UI座標をワールド座標に変換する
    /// </summary>
    Vector3 GetWorldPoint(Vector3 UIPos)
    {
        Vector3 point = UIPos + Vector3.forward * MousePos_z;
        //Debug.Log(point + "+" + Camera.main.ScreenToWorldPoint(point));
        return Camera.main.ScreenToWorldPoint(point);
    }

    /// <summary>
    /// 指定したマウスボタンに応じて、パラメータの更新とイベント呼び出しを行う。
    /// </summary>
    void MouseInputParameter(int num,ref MouseParameter parameter)
    {
        // マウスボタンを押した瞬間
        if (Input.GetMouseButtonDown(num))
        {
            parameter.mouseUIDownPos = GetTouchClamp();// UI座標を取得
            parameter.mouseWorldDownPos = GetWorldPoint(parameter.mouseUIDownPos);// ワールド座標に変換

            ClickDownInvoke(num,parameter.mouseWorldDownPos);
        }
        // マウスボタンを押している間
        if (Input.GetMouseButton(num))
        {
            parameter.mouseUIPos = GetTouchClamp();// 現在のUI座標
            parameter.mouseWorldPos = GetWorldPoint(parameter.mouseUIPos);// 現在のワールド座標

            ClickInvoke(num,parameter.mouseWorldPos);
        }
        // マウスボタンを離したとき
        if (Input.GetMouseButtonUp(num))
        {
            ClickUpInvoke(num, parameter.mouseWorldPos);
            // リセット
            parameter.ResetMousePos();
        }
    }

    /// <summary>
    /// マウス位置をUI範囲内に制限した座標を返す。
    /// （画面外に出ないようにクリップ）
    /// </summary>
    Vector3 GetTouchClamp()
    {
        Vector3 ClampPosition = Input.mousePosition;

        // 横方向（X）と縦方向（Y）をそれぞれ画面サイズ内に収める
        ClampPosition.y = Mathf.Clamp(ClampPosition.y, ClampMin, UIheightMax);
        ClampPosition.x = Mathf.Clamp(ClampPosition.x, ClampMin, UIwidthMax);

        //Debug.Log(Input.mousePosition + "+" + ClampPosition);
        return ClampPosition;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初期化処理。左右のマウスパラメータを初期化。
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
        // 左クリックの処理
        MouseInputParameter(LeftInputNum,ref LeftParameter);
        // 右クリックの処理
        MouseInputParameter(RightInputNum,ref RightParameter);
    }  
}
