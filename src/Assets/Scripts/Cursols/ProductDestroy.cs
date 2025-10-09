using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductDestroy : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer DestroySpriteShadow;

    [SerializeField]
    Color enabledColor = Color.blue;

    [SerializeField]
    Color disabledColor = Color.cyan;

    [SerializeField]
    float fixScaling = 16f;

    [SerializeField]
    AudioClip Clip;

    bool DestroyFlag; // 現在破壊モード中かどうか
    bool OnClickUI; // UI上をクリックしているかどうか

    public void DestorySetUp() => DestroyFlag = true;

    /// <summary>
    /// 破壊可能状態をキャンセルする
    /// </summary>
    public void CancelDestroy() => ResetDestroy();

    /// <summary>
    /// 破壊可能な状態かを外部が確認するためのフラグ（カーソルの非表示制御などに利用）
    /// </summary>
    public bool IsDestroyed() => !DestroyFlag;

    public void InputRegister(MouseController input)
    {
        // マウス入力イベントを登録
        input.LeftDownEvent += ClickSpriteRenderer;
        input.LeftClickEvent += SetCreateTransform;
        input.LeftUpEvent += DestroyProduct;
    }

    /// <summary>
    /// ワールド座標を整数のグリッド座標へ変換
    /// </summary>
    Vector2Int Cursol2DInt(Vector3 mouseWorldDownPos) => new Vector2Int()
    {
        x = Mathf.RoundToInt(mouseWorldDownPos.x),
        y = Mathf.RoundToInt(mouseWorldDownPos.y)
    };

    void ResetDestroy()
    {
        DestroyFlag = false;
        OnClickUI = false;
        ResetSpritePos();// スプライトを画面外に移動
    }

    /// <summary>
    /// スプライトを画面外に移動させて非表示にする
    /// </summary>
    void ResetSpritePos()
    {
        int OutCameraPosValue = -50;

        transform.position = new Vector3Int()
        {
            x = OutCameraPosValue,
            y = OutCameraPosValue,
            z = 0
        };

        // スケールを初期化
        transform.localScale = Vector3.one * fixScaling;
    }

    /// <summary>
    /// 対象マスがマップ範囲内かどうかを判定する
    /// </summary>
    /// <returns>範囲内ならtrue</returns>
    bool IsInGridMap(Vector3 mouseWorldPos)
    {
        Vector2Int GridPos = Cursol2DInt(mouseWorldPos);

        return GridMapManager.Instance.IsInBounds(GridPos);
    }

    /// <summary>
    /// 対象マスに建物があるかどうかを判定する
    /// </summary>
    /// <returns>建物があればtrue</returns>
    bool IsExistBuilding(Vector2Int cursolPos)
    {
        var gridMap = GridMapManager.Instance;

        return !(gridMap.GetCell(cursolPos).GetBuilding() == null);
    }

    /// <summary>
    /// 破壊可能な建物かどうか判定
    /// </summary>
    /// <param name="type">建物カデゴリ</param>
    /// <returns>可能ならtrue</returns>
    bool CanDestroyBuildingType(BuildType type)
    {
        return
            type == BuildType.Belt ||
            type == BuildType.MultiBelt ||
            type == BuildType.Production ||
            type == BuildType.Processing ||
            type == BuildType.Turret ||
            type == BuildType.Wall;
    }

    /// <summary>
    /// 左クリックをした時、スプライトを移動させる
    /// </summary>
    void ClickSpriteRenderer(Vector3 mouseWorldDownPos)
    {
        Debug.Log("a");
        if (!DestroyFlag)
            return;
        Debug.Log("b");
        if (EventSystem.current.IsPointerOverGameObject())
        {
            OnClickUI = true;// UI上をクリックしてた場合キャンセル
            return;
        }

        // マウスのワールド座標を整数グリッドに変換し、スプライト位置を更新
        Vector2Int cursol2DInt = Cursol2DInt(mouseWorldDownPos);

        transform.position = new Vector3Int()
        {
            x = cursol2DInt.x,
            y = cursol2DInt.y,
            z = 0
        };
    }

    /// <summary>
    /// 左クリックした位置に、破壊可能かどうかを確認してスプライトの色を変える
    /// </summary>
    void SetCreateTransform(Vector3 mouseWorldPos)
    {
        if (!DestroyFlag || OnClickUI)
            return;

        Vector2Int cursol2DInt = Cursol2DInt(mouseWorldPos);

        bool inMap = IsInGridMap(mouseWorldPos);// マウス位置がグリッドマップの外か
        bool Exist = inMap && IsExistBuilding(cursol2DInt);// グリッドマップ内かつ生成可能なタイルか

        var cell = GridMapManager.Instance.GetCell(cursol2DInt);// セル情報を取得

        bool CanDestroy = CanDestroyBuildingType(cell.GridCellType);// 破壊可能な建物カデゴリか

        // 条件を満たしていれば「有効色」、そうでなければ「無効色」
        DestroySpriteShadow.color = (inMap && Exist && CanDestroy) ? enabledColor : disabledColor;

        // スケールを変更する
        // 建物がない場合は、1*1*1サイズに
        transform.localScale = cell.GetBuildingSize() * fixScaling;

        Vector3 SpritePosition;

        // 建物がない場合
        if (cell.IsNoneCelltype())
        {
            // 影スプライトをカーソル位置に移動
            SpritePosition = new()
            {
                x = cursol2DInt.x,
                y = cursol2DInt.y,
                z = 0
            };
        }
        // 建物がある場合
        else
        {
            Vector2 BuildingSenter = cell.GetBuildingSenterPos();

            // 影スプライトを建物の中心の位置に移動
            SpritePosition = new()
            {
                x = BuildingSenter.x,
                y = BuildingSenter.y,
                z = 0
            };
        }

        transform.position = SpritePosition;
    }

    /// <summary>
    /// 左ボタン離したとき、条件を満たせば実際に生成処理を実行
    /// </summary>
    void DestroyProduct(Vector3 mouseWorldUpPos)
    {
        // 現在破壊モード中判定
        if (!DestroyFlag)
        {
            return;
        }
        // UIの上でマウスが離された場合は無視する
        if (OnClickUI || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        // マウス位置がグリッドマップの外なら生成できない
        if (!IsInGridMap(mouseWorldUpPos))
        {
            return;
        }

        // マウスのワールド座標を整数グリッド座標へ変換
        Vector2Int cursol2DInt = Cursol2DInt(mouseWorldUpPos);

        // 生成可能なタイルかどうかを判定
        if (!IsExistBuilding(cursol2DInt))
        {
            return;
        }

        // セル情報を取得
        var cell = GridMapManager.Instance.GetCell(cursol2DInt);

        // 破壊可能な建物カデゴリか判定
        if (!CanDestroyBuildingType(cell.GridCellType))
        {
            return;
        }

        AudioManager.instance.isPlaySE(Clip);// SE再生

        // 破壊するオブジェクトを取得
        GameObject destroyTarget = GridMapManager.Instance.GetCell(cursol2DInt).GridObject;

      　// 内部クラスの削除
        GridMapManager.Instance.DestroyContent(cursol2DInt);

        // オブジェクトの削除
        Destroy(destroyTarget);

        ResetDestroy();
    }


    void Start()
    {
        // 初期化処理
        DestroyFlag = false;
        OnClickUI = false; 
    }
}
