using UnityEngine;

public class TargetCursol : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;// カーソル表示対象のTransform

    [SerializeField]
    float fixScaling  = 0.5f; // スケールを調整するパラメータ

    [SerializeField]
    SpriteRenderer CursolSprite;

    [SerializeField]
    ProductDestroy productDestroy;

    [SerializeField]
    ProductCreate productCreate;

    // 建物の生成・破壊をしていないかどうかを調べる
    bool IsNoneProductFunc() => productCreate.IsCreated() && productDestroy.IsDestroyed();

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

        // 対応するグリッドセルを取得
        var gridCell = GridMapManager.Instance.GetCell(mapPos2DInt);

        // スケールを変更する
        // 建物がない場合は、1*1*1サイズに
        transform.localScale = gridCell.GetBuildingSize() * fixScaling;

        Vector3 SpritePosition = new()
        {
            x = mapPos2DInt.x,
            y = mapPos2DInt.y,
            z = 0
        };

        // そのマスに建物がある場合は建物の中央に移動
        if (!gridCell.IsNoneCelltype())
        {
            Vector2 BuildingSenter = gridCell.GetBuildingSenterPos();

            SpritePosition = new Vector3()
            {
                x = BuildingSenter.x,
                y = BuildingSenter.y,
                z = 0
            };
        }

        // 対象の位置にカーソルを移動
        targetTransform.position = SpritePosition;
    }

    void LateUpdate()
    {
        CursolSprite.enabled = IsNoneProductFunc();
    }
}
