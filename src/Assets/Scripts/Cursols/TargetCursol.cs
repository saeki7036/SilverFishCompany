using UnityEngine;

public class TargetCursol : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;// カーソル表示対象のTransform

    // マップの最大サイズ（インスタンス経由）
    Vector2Int maxMapSize => GridMapManager.Instance.MaxMapSize;

    // グリッドの補正スケール（グリッド座標境界の調整に使用）
    float GridAdjustScale => GridMapManager.Instance.GridAdjustScale();

    // Clamp処理の最小値
    const int ClampMin = 0;

    //範囲外の場合にカメラ外に移動させるための数値
    static readonly Vector3Int OutRangePos = new(9999, 9999, 0);

    // インデックスからの取得のため -1 をしている
    Vector2Int MaxMapIndex => maxMapSize - Vector2Int.one;

    // 初期スケール
    Vector3 StartCorsolScale;

    public void InputRegister(MouseController input)
    {
        // 入力イベントを登録する
        input.LeftDownEvent += SetTargetTransform;
    }

    /// <summary>
    /// マウス位置がマップ範囲内かを判定する
    /// </summary>
    /// <param name="mouseWorldDownPos">ワールド座標でのマウス位置</param>
    /// <returns>範囲内かどうか</returns>
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

    /// <summary>
    /// マウスクリック位置にカーソルを移動させる処理
    /// </summary>
    /// <param name="mouseWorldDownPos">マウスのワールド座標</param>
    void SetTargetTransform(Vector3 mouseWorldDownPos)
    {
        // 範囲外ならカーソルを画面外へ移動
        if (!IsInGridMap(mouseWorldDownPos))
        {
            targetTransform.position = OutRangePos;
            return;
        }

        // マップ座標に変換し、範囲内にClamp
        Vector2Int mapPos2DInt = new()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.x), ClampMin, MaxMapIndex.x),
            y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldDownPos.y), ClampMin, MaxMapIndex.y),
        };

        // 対応するグリッドオブジェクトを取得
        GameObject gridObject = GridMapManager.Instance.GetCell(mapPos2DInt).GridObject;

        // そのマスにオブジェクトが無い場合は座標だけ移動
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

        // 対象オブジェクトの位置にカーソルを移動
        Vector3 contentObjectWorldPosition = gridObject.transform.position;

        targetTransform.position = contentObjectWorldPosition;

        // 対象オブジェクトのスケールを反映
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
        // 初期化処理：カーソルの初期スケールを保存
        StartCorsolScale = transform.localScale;
    }
}
