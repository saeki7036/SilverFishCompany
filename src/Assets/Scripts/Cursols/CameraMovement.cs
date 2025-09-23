using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    float CameraBaseMoveSpeed = 3f;// カメラ移動の基本速度（差分に加算される）

    [SerializeField]
    Vector2 ThresholdValue = new(5f, 3f);// マウスとカメラ位置の差の閾値（これを超えたら移動）

    // マップの最大サイズ（右上の制限位置）
    Vector2Int MaxMapSize => GridMapManager.Instance.MaxMapSize;

    const float ClampMin = 0f;// マップの最小位置（左下の制限）


    public void InputRegister(MouseController input)
    {
        // 右クリックイベントを登録する
        input.RightClickEvent += CameraMove;
    }

    /// <summary>
    /// 1軸分の移動方向を取得。
    /// カメラ位置(thisPos)とマウス位置(mousePos)の差が閾値(thresholdValue)より大きければ -1 or 1 を返し、
    /// それ以外は 0（移動なし）を返す。
    /// </summary>
    int GetMoveDirectionAxis(float thisPos,float mousePos,float thresholdValue)
    {
        //Debug.Log(thisPos - mousePos);
        //Mathf.RoundToInt(thisPos - mousePos)
        float diff = thisPos - mousePos;

        // マウスがカメラの左（または下）に大きく離れているのでカメラを逆方向に移動
        if (diff > thresholdValue)
            return -1;

        // マウスがカメラの右（または上）に大きく離れているのでカメラを正方向に移動
        if (diff < -thresholdValue)
            return 1;
        // 閾値内のため移動なし
        return 0;
    }

    /// <summary>
    /// 1軸分の移動速度を取得。
    /// マウスとカメラの距離の絶対値に基本速度を加算し、
    /// マウスが離れているほど速くなる計算。
    /// </summary>
    float GetMoveSpeedAxis(float thisPos, float mousePos)
    {
        return (Mathf.Abs(thisPos - mousePos) + CameraBaseMoveSpeed);
    }

    /// <summary>
    /// 右クリックのワールド座標に応じてカメラを移動させる。
    /// </summary>
    void CameraMove(Vector3 mouseWorldPos)
    {
        // X軸・Y軸それぞれの移動方向を計算
        Vector2Int moveDirection = new()
        {
            x = GetMoveDirectionAxis(transform.position.x, mouseWorldPos.x,ThresholdValue.x),
            y = GetMoveDirectionAxis(transform.position.y, mouseWorldPos.y,ThresholdValue.y),
        };

        // X軸・Y軸それぞれの移動速度を計算
        Vector2 moveSpeed = new()
        {
            x = GetMoveSpeedAxis(transform.position.x, mouseWorldPos.x),
            y = GetMoveSpeedAxis(transform.position.y, mouseWorldPos.y),
        };

        // 実際のカメラ移動後の座標を計算（時間差でスムーズに）
        Vector3 afterMovePos = new()
        {
            x = transform.position.x + (moveDirection.x * moveSpeed.x * Time.deltaTime),
            y = transform.position.y + (moveDirection.y * moveSpeed.y * Time.deltaTime),
            z = transform.position.z,// Z軸は固定（カメラの高さ等）
        };

        // カメラ位置を更新
        transform.position = afterMovePos;

        // 移動後の位置をマップ内に制限（カメラがマップ外に行かないように）
        PositionClamp();
    }

    /// <summary>
    /// カメラの位置がマップ範囲内に収まるようにClampする処理。
    /// </summary>
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
}
