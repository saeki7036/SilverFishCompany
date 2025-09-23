using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductUICreate : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer contentSpriteShadow;// 生成予定オブジェクトの影表示

    [SerializeField]
    BeltDrawing beltDrawing;// ベルト生成の制御

    [SerializeField]
    Color enabledColor = Color.blue;// 設置可能状態の色

    [SerializeField]
    Color disabledColor = Color.cyan;// 設置不可状態の色

    [SerializeField]
    AudioClip Clip;// 生成時

    GameObject contentPrehab; // 生成対象のPrefab
    GridContent gridContent; // 対象Prefabに付随する内容情報
    bool CreateFlag; // 現在生成モード中かどうか
    List<ItemRequest> requests; // 生成に必要なアイテムリスト

    float GridAdjustScale => GridMapManager.Instance.GridAdjustScale();

    Vector2Int MaxMapSize => GridMapManager.Instance.mapSize;

    /// <summary>
    /// 生成可能な状態かを外部が確認するためのフラグ（カーソルの非表示制御などに利用）
    /// </summary>
    public bool IsCreated()=> !CreateFlag && !beltDrawing.GetDrawFlag();

    /// <summary>
    /// スプライトを画面外に移動させて非表示にする
    /// </summary>
    void ResetSpritePos()
    {
        int OutCameraPosValue = -10;

        transform.position = new Vector3Int()
        {
            x = OutCameraPosValue,
            y = OutCameraPosValue,
            z = 0
        };
    }

    /// <summary>
    /// UI選択時に生成対象情報を登録する（Prefabとスプライト）
    /// </summary>
    public void SetCreateContent(List<ItemRequest> list,GameObject gameObject = null, Sprite sprite = null)
    {
        // プレハブとスプライトが揃っていれば生成準備
        CreateFlag = (gameObject != null && sprite != null);

        requests = new List<ItemRequest>(list);
        contentPrehab = gameObject;
        contentSpriteShadow.sprite = sprite;

        if (CreateFlag == false)
            return;

        // プレハブにアタッチされたGridContentを取得（サイズや属性情報）
        gridContent = contentPrehab.GetComponent<GridContent>();

        BeltSetting(); // ベルトかどうかに応じて挙動変更
    }

    /// <summary>
    /// ベルトの場合、描画状態や生成可否を切り替える
    /// </summary>
    void BeltSetting()
    {
        if (gridContent.IsBelt())
        {
            // ベルトの場合、別描画を担当するため自動描画は停止
            beltDrawing.SetDrawFlag(true);
            contentSpriteShadow.enabled = false;
            CreateFlag = false;
        }
        else
        {
            beltDrawing.SetDrawFlag(false);
            contentSpriteShadow.enabled = true;
        }   
    }

    /// <summary>
    /// 生成中の状態をリセットする
    /// </summary>
    void EmptyContent()
    {
        // スプライトと内容情報をクリア
        contentSpriteShadow.enabled = false;
        contentSpriteShadow.sprite = null;
        gridContent = null;
        beltDrawing.SetDrawFlag(false);
        CreateFlag = false;
        requests = new List<ItemRequest>();
    }

    public void InputRegister(MouseController input)
    {
        // マウス入力イベントを登録
        input.LeftDownEvent += DragSpriteRenderer;
        input.LeftClickEvent += SetCreateTransform;
        input.LeftUpEvent += CreateProduct;
    }

    /// <summary>
    /// アイテム要求を満たしているか
    /// </summary>
    bool CheckItemRequests() => ItemManager.Instance.CanConsumeAll(requests);

    /// <summary>
    /// アイテム要求分を消費する
    /// </summary>
    bool ConsumeItemRequests() => ItemManager.Instance.ItemConsumeAll(requests);

    /// <summary>
    /// ワールド座標を整数のグリッド座標へ変換
    /// </summary>
    Vector2Int Cursol2DInt(Vector3 mouseWorldDownPos) => new Vector2Int()
    {
        x = Mathf.RoundToInt(mouseWorldDownPos.x),
        y = Mathf.RoundToInt(mouseWorldDownPos.y)
    };

    // <summary>
    /// 生成対象がマップ範囲内かどうかを判定する
    /// </summary>
    bool IsInGridMap(Vector3 mouseWorldPos)
    {
        Vector2Int GridRange = gridContent.GetContent().GridSize - Vector2Int.one;  

        if (mouseWorldPos.x < -GridAdjustScale ||
            MaxMapSize.x - GridAdjustScale < mouseWorldPos.x + GridRange.x)
            return false;

        if (mouseWorldPos.y < -GridAdjustScale ||
            MaxMapSize.y - GridAdjustScale < mouseWorldPos.y + GridRange.y)
            return false;

        return true;
    }

    /// <summary>
    /// 指定位置に生成可能なタイルが存在するかどうかを判定
    /// </summary>
    bool IsCanCreateTile(Vector2Int cursolPos)
    {
        // 地形タイルを取得
        TileType tileType = gridContent.GetContent().CanCreateTileType();

        // Noneの場合基本的どの地形の上でも問題ないので true
        if(tileType == TileType.None)
            return true;

        var gridMap = GridMapManager.Instance;

        // 範囲内すべてのマスに対して条件を満たしているか確認
        for (int x = cursolPos.x; x < cursolPos.x + gridContent.GetContent().GridSize.x; x++)
        {
            for (int y = cursolPos.y; y < cursolPos.y + gridContent.GetContent().GridSize.y; y++)
            {
                Vector2Int vector2Int = new Vector2Int()
                {
                    x = x,
                    y = y,
                };

                // グリッドマップの範囲内か
                if(!gridMap.IsInBounds(vector2Int))
                    return false;

                //地形タイルが同じか判定
                if(!gridMap.GetCell(vector2Int).SameTileType(tileType))
                    return false;
            }
        }
     
        return true;
    }

    /// <summary>
    /// 左クリックドラッグ時、生成スプライトを移動させる
    /// </summary>
    void DragSpriteRenderer(Vector3 mouseWorldDownPos)
    {
        if (!CreateFlag)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            CreateFlag = false;// UI上をクリックしてた場合キャンセル
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
    /// 左クリックした位置に、設置可能かどうかを確認してスプライトの色を変える
    /// </summary>
    void SetCreateTransform(Vector3 mouseWorldPos)
    {
        if (!CreateFlag)
            return;

        Vector2Int cursol2DInt = Cursol2DInt(mouseWorldPos);
        bool inMap = IsInGridMap(mouseWorldPos);// マウス位置がグリッドマップの外か
        bool canCreate = inMap && IsCanCreateTile(cursol2DInt);// グリッドマップ内かつ生成可能なタイルか
        bool hasItems = CheckItemRequests();// 素材不足でないか

        // 条件を満たしていれば「有効色」、そうでなければ「無効色」
        contentSpriteShadow.color = (inMap && canCreate && hasItems) ? enabledColor : disabledColor;

        // 影スプライトを設置候補位置に表示
        transform.position = new Vector3Int()
        {
            x = cursol2DInt.x,
            y = cursol2DInt.y,
            z = 0
        };
    }

    /// <summary>
    /// 左ボタン離したとき、条件を満たせば実際に生成処理を実行
    /// </summary>
    void CreateProduct(Vector3 mouseWorldUpPos)
    {
        // 現在生成モード中判定
        if (!CreateFlag) 
        {
            return;
        }
        // UIの上でマウスが離された場合は無視する
        if (EventSystem.current.IsPointerOverGameObject())
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
        if (!IsCanCreateTile(cursol2DInt))
        {
            return;
        }
        if(!CheckItemRequests() || !ConsumeItemRequests())
        {
            return;// 素材不足や消費失敗があれば中断
        }

        transform.position = new Vector3Int()
        {
            x = cursol2DInt.x,
            y = cursol2DInt.y,
            z = 0
        };

        // Prefabを生成
        Instantiate(contentPrehab, transform.position, Quaternion.identity);

        AudioManager.instance.isPlaySE(Clip);// SE再生

        EmptyContent();// 内部状態クリア

        ResetSpritePos();// スプライトを画面外に移動
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初期化処理
        CreateFlag = false;
        requests = new List<ItemRequest>();
    }
}
