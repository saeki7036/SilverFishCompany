using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeltDrawing : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;// ベルト経路の線を描画するLineRenderer

    [SerializeField]
    Gradient scsessGradient;// 成功時に表示される経路色（アイテム条件・経路OK）

    [SerializeField]
    Gradient failedGradient;// 失敗時に表示される経路色（建物と干渉やアイテム不足）

    [SerializeField]
    GameObject BeltPrefab;// ベルト本体のPrefab

    [SerializeField]
    GameObject StartPosIcon;// ベルト始点を示すアイコン

    [SerializeField]
    SpriteRenderer StratIconSprite;// 始点アイコンの色変更用SpriteRenderer

    [SerializeField]
    GameObject EndPosIcon;// ベルト終点を示すアイコン

    [SerializeField]
    SpriteRenderer EndIconSprite;// 終点アイコンの色変更用SpriteRenderer

    [SerializeField]
    List<ItemRequest> requests;// ベルト作成時に必要なアイテムリスト

    [SerializeField]
    AudioClip Clip; // ベルト設置時に再生される効果音

    Vector3Int currentPos; // 触ったグリッドの場所
    bool OnGridMap;// 左クリックした場所がグリッドマップ内かのフラグ
    bool IsNoProblemRoute;// 経路がすでにある建物と干渉していないか
    bool DrawFlag;// ベルト描画モードの有効/無効フラグ
    bool ItemFlag;// 必要なアイテムが足りているかのフラグ

    // 現在選択中のグリッド経路（始点〜終点）
    List<Vector3Int> SelectedPosList = new List<Vector3Int>();

    // 各グリッド座標が選択リストの何番目かを記録（巻き戻し用）
    Dictionary<Vector3Int, int> posIndexMap = new Dictionary<Vector3Int, int>();

    const int ClampMin = 0;// マップ座標の最小値

    // マップサイズの最大値（外部参照）
    Vector2Int maxMapSize => GridMapManager.Instance.MaxMapSize;

    // マップサイズ調整スケール（外部参照）
    float GridAdjustScale => GridMapManager.Instance.GridAdjustScale();

    public void InputRegister(MouseController input)
    {
        // イベント登録
        input.LeftDownEvent += BeltDrawSetup;
        input.LeftClickEvent += DrawingBelt;
        input.LeftUpEvent += DrowBeltGenerate;
    }

    public bool GetDrawFlag() => DrawFlag;
    public void SetDrawFlag(bool flag) => DrawFlag = flag;

    /// <summary>
    /// ワールド座標をグリッドマップ内の整数座標に変換する
    /// マップサイズの範囲内にクランプされる
    /// </summary>
    Vector3Int GetMapGridInt(Vector3 mouseWorldPos)
    {
        // インデックスからの取得のため -1 をしている
        Vector2Int maxMapIndex = maxMapSize - Vector2Int.one;

        return new()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldPos.x), ClampMin, maxMapIndex.x),
            y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldPos.y), ClampMin, maxMapIndex.y),
            z = 0,
        };
    }

    /// <summary>
    /// 経路に必要なアイテムがすべて存在するかを確認する
    /// </summary>
    bool CheckItemRequests()
    {
        // ベルトに必要な個数を計算（開始・終了地点を除く）
        int requestValue = Mathf.Max(0, SelectedPosList.Count - 2);

        // 各リクエストアイテムについて在庫をチェック
        foreach (var request in requests)
        {
             int value = request.GetValue() * requestValue;
            bool consume = ItemManager.Instance.CanConsumeItem(request.GetCategory(), request.GetLevel(), value);

            if(!consume)
                return false;
        }

        return true;
    }
    /// <summary>
    /// 経路に必要なアイテムを実際に消費する（可能でなければ失敗）
    /// </summary>
    bool ConsumeItemRequests()
    {
        // 消費可能かチェック
        if (!CheckItemRequests())
            return false;
        // ベルトに必要な個数を計算
        int requestValue = Mathf.Max(0, SelectedPosList.Count - 2);
        // 実際にアイテムを消費
        foreach (var request in requests)
        {
            int value = request.GetValue() * requestValue;
            bool consume = ItemManager.Instance.ItemConsume(request.GetCategory(), request.GetLevel(), value);

            if (!consume)
                return false;
        }

        return true;
    }


    /// <summary>
    /// 座標がグリッドマップ内に収まっているか確認する
    /// </summary>
    bool IsInGridMap(Vector3 mouseWorldDownPos)
    {
        // X座標の範囲チェック
        if (mouseWorldDownPos.x < -GridAdjustScale || 
            maxMapSize.x - GridAdjustScale < mouseWorldDownPos.x)
            return false;

        // Y座標の範囲チェック
        if (mouseWorldDownPos.y < -GridAdjustScale || 
            maxMapSize.y - GridAdjustScale < mouseWorldDownPos.y)
            return false;

        return true;
    }

    /// <summary>
    /// 2つのグリッド座標を縦横の動きのみで対角線に近づけながら繋ぐ経路を生成する処理
    /// </summary>
    List<Vector3Int> CorrectionBeltLine(Vector3Int endPos, Vector3Int startPos)
    {
        // ブレゼンハムの線分アルゴリズム(Bresenham's Line Algorithm)を斜め移動禁止で実装
        List<Vector3Int> path = new List<Vector3Int>();

        // 移動量の絶対値（距離）
        int distance_x = Mathf.Abs(endPos.x - startPos.x);
        int distance_y = Mathf.Abs(endPos.y - startPos.y);

        // x方向・y方向に進むべき向き（+1 or -1）
        int step_x = startPos.x < endPos.x ? 1 : -1;
        int step_y = startPos.y < endPos.y ? 1 : -1;

        // 開始座標のコピー
        int current_x = startPos.x;
        int current_y = startPos.y;

        // 誤差の初期値（x方向とy方向の距離差を使う）
        int errorValue = distance_x - distance_y;

        // 終点に到達するまでループ
        while (current_x != endPos.x || current_y != endPos.y)
        {          
            // 誤差を2倍して比較（整数で斜めの傾きを近似）
            int errorDouble = errorValue * 2;

            // 現在地に1マスずつ追加（縦または横のいずれか）
            // ----------------------------------------
            // 1マス進む条件
            // ----------------------------------------
            // 「誤差 × 2」が -distance_y 又は distance_x と比較して
            // 別方向に進むべき傾きに対して、進む方向のズレがまだ許容範囲内なので、
            // 進む方向に進んでも斜めの線に近づけると判断する。
            //
            // つまり：
            //  - 誤差が大きくなる前に進む
            //  - 今は別進む方向より進む方向を優先すべき段階
            //
            // → 1マス移動して、誤差から反対方向分を差し引く
            if (errorDouble > -distance_y)
            {
                // x方向に1マス移動

                // Y成分を引いて誤差を更新（Xへ進んだ分ズレる）
                errorValue -= distance_y;

                // X座標を1マス進める（左か右）
                current_x += step_x;

                // 新しい位置をリストに追加
                path.Add(new Vector3Int(current_x, current_y, 0));
            }
            else if (errorDouble < distance_x)
            {
                // y方向に1マス移動

                // X成分を加えて誤差を更新（Yへ進んだ分ズレる）
                errorValue += distance_x;

                // Y座標を1マス進める（上か下）
                current_y += step_y;

                // 新しい位置をリストに追加
                path.Add(new Vector3Int(current_x, current_y, 0));
            }
        }

        return path;
    }

    // <summary>
    /// ベルト描画の開始処理（マウス左ボタン押下時）
    /// グリッド座標の初期化と開始地点の設定を行う
    /// </summary>
    void BeltDrawSetup(Vector3 mouseWorldDownPos)
    {
        // リストとマップを初期化
        SelectedPosList = new List<Vector3Int>();
        posIndexMap = new Dictionary<Vector3Int, int>();

        // ラインレンダラーを全消去（描画をリセット）
        lineRenderer.positionCount = 0;

        // マウス位置がグリッドマップ内かチェック
        OnGridMap = IsInGridMap(mouseWorldDownPos);

        // UIオブジェクト上でクリックした場合は処理しない
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 描画モード有効 かつ グリッドマップ内の場合
        if (DrawFlag && OnGridMap)
        {
            // 開始・終了アイコンを表示
            StartPosIcon.SetActive(true);
            EndPosIcon.SetActive(true);

            // マウス位置をグリッド座標に変換
            Vector3Int gridPos = GetMapGridInt(mouseWorldDownPos);

            // アイコンを開始位置に配置
            StartPosIcon.transform.position = gridPos;
            EndPosIcon.transform.position = gridPos;

            // 開始地点を選択リストに追加
            posIndexMap.Add(gridPos, SelectedPosList.Count);
            SelectedPosList.Add(gridPos);
            currentPos = gridPos;
        } 
    }

    /// <summary>
    /// ベルト（選択ライン）を描画する処理。マウスのワールド座標を元に選択ラインの更新を行う。
    /// 経路上の各グリッド座標を保存し、巻き戻し処理にも対応する。
    /// 途中で経路が既に存在する建物に接触していないかなどを確認する。
    /// </summary>
    void DrawingBelt(Vector3 mouseWorldPos)
    {
        // 描画モード無効 または グリッドマップ外の場合は処理しない
        if (!DrawFlag || !OnGridMap)     
            return;
        
        // マウスのワールド座標をグリッド座標（整数）に変換
        Vector3Int gridPos = GetMapGridInt(mouseWorldPos);

        // すでに何らかの位置が選択されてる場合に処理を実行
        if (SelectedPosList.Count != 0)
        {
            // アイテム在庫チェック
            ItemFlag = CheckItemRequests();

            // 今回の位置が前回の位置と同じ場合に処理を終了
            if (gridPos == currentPos)       
                return;

            // 今回の位置から対角線をジグザグに補正した経路を取得
            List<Vector3Int> correctionPathList = CorrectionBeltLine(gridPos, currentPos);

            // 経路1マスごとににグリッド座標を処理
            foreach (Vector3Int nextPos in correctionPathList)
            {
                // すでに選択リスト上にこのグリッド座標が含まれていた場合（ルートを巻き戻すような操作）
                if (posIndexMap.TryGetValue(nextPos, out int count))
                {
                    // 該当位置以降のデータをすべて削除する（巻き戻し）
                    for (int i = SelectedPosList.Count - 1; i >= count && i >= 0; i--)
                    {
                        Vector3Int removalTargetPos = SelectedPosList[i];
                        SelectedPosList.RemoveAt(i);
                        posIndexMap.Remove(removalTargetPos);
                    }
                }

                // まだ選択リストに含まれていない座標の場合、新たに追加
                if (!posIndexMap.ContainsKey(nextPos))
                {
                    // 座標をマップに登録し、選択リストにも追加
                    posIndexMap.Add(nextPos, SelectedPosList.Count);
                    SelectedPosList.Add(nextPos);

                    // 現在位置を更新
                    currentPos = nextPos;
                }
            }
            // アイテム在庫を再チェック
            ItemFlag = CheckItemRequests();

            // 経路が問題ないか確認
            IsNoProblemRoute = RouteProblemCheck();

            // ラインレンダラーを更新して描画を反映
            UpdateLineRenderer();

            // 終点の位置にオブジェクトを移動する
            EndPosIcon.transform.position = gridPos;
        }
    }

    /// <summary>
    /// マウスの左ボタンを離したときにベルトの生成を行う
    /// 選択された経路が有効で、かつ必要なリソースがあればBeltオブジェクトを生成して配置する
    /// </summary>
    void DrowBeltGenerate(Vector3 mouseWorldUpPos)
    {
        // 描画モードが無効なら処理しない
        if (!DrawFlag)
            return;

        // ベルトプレハブが設定されていない場合は処理しない
        if (BeltPrefab == null)
            return;

        // LineRendererの描画を初期化
        lineRenderer.positionCount = 0;

        // アイコンを非表示にする
        StartPosIcon.SetActive(false);
        EndPosIcon.SetActive(false);

        // 経路に既に建物があるなら生成せずに終了
        if(!IsNoProblemRoute)
        {
            return;
        }

        // 先頭位置と終点位置を除くため要素数3以上なければ処理を終了させる
        if (SelectedPosList.Count <= 2)
        {
            Debug.Log("選択セル：" + SelectedPosList.Count);
            return;
        }

        // Item個数が足りなければ終了させる
        if(!ConsumeItemRequests())
        {
            return;
        }

        // 効果音を再生
        AudioManager.instance.isPlaySE(Clip);

        // 生成するベルトオブジェクトのリスト
        List<GameObject> BeltList = new List<GameObject>();

        // 開始地点と終了地点を保存
        Vector3Int StratPos = SelectedPosList.First();
        Vector3Int EndPos = SelectedPosList.Last();

        // 終点から削除
        SelectedPosList.RemoveAt(SelectedPosList.Count - 1); // 終点を削除
        SelectedPosList.RemoveAt(0); // 先頭を削除

        // 中間地点にベルトオブジェクトを生成
        foreach (Vector3Int posInt in SelectedPosList)
        {
            GameObject BeltObject = Instantiate(BeltPrefab,posInt,Quaternion.identity);
            BeltList.Add(BeltObject);
        }

        // グリッドマップにベルト情報を設定
        GridMapManager.Instance.BeltSetting(SelectedPosList, BeltList, StratPos, EndPos);

        // 描画モードを終了
        DrawFlag = false;
    }

    /// <summary>
    /// ベルト経路上に既存の建物がないかをチェックする（先頭・終点を除く）
    /// </summary>
    /// <returns>true：問題なし / false：建物と干渉</returns>
    bool RouteProblemCheck()
    {
        // 先頭要素と終点要素を除いて既に建物があるか確認
        for (int i = 1; i < SelectedPosList.Count - 1; i++)
        {
            // Vector3IntからVector2Intに変換
            Vector2Int vector2Int = new Vector2Int()
            {
                x = SelectedPosList[i].x,
                y = SelectedPosList[i].y,
            };

            // セルが空かどうかチェック
            if (GridMapManager.Instance.GetCell(vector2Int).IsNoneCelltype())
            {
                continue;
            }
            else// 建物がある場合は問題あり
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// ラインレンダラーを更新して描画をする処理
    /// </summary>
    void UpdateLineRenderer()
    {
        // 選択された地点数分の頂点を設定
        lineRenderer.positionCount = SelectedPosList.Count;

        // Vector3Int → Vector3 変換しつつ設定
        for (int i = 0; i < SelectedPosList.Count; i++)
        {
            // 高さ調整したい場合はZに+0.5fなどする
            lineRenderer.SetPosition(i, SelectedPosList[i]);
        }

        //経路を生成出来るかで色変更
        GradientSetting();
    }

    /// <summary>
    /// 経路の状態に応じて、ラインの色やアイコンの色を変更する
    /// 問題なし：成功グラデーション、問題あり：失敗グラデーション
    /// </summary>
    void GradientSetting()
    {
        float GradientFirstTime = 0f;
        float GradientLastTime = 1f;

        // 経路チェックとアイテムチェックの両方をクリアしているか
        bool RouteCheck = IsNoProblemRoute && ItemFlag;
        Gradient gradient = RouteCheck ? scsessGradient : failedGradient;

        // ラインとアイコンの色を設定
        lineRenderer.colorGradient = gradient;
        StratIconSprite.color = gradient.Evaluate(GradientFirstTime);
        EndIconSprite.color = gradient.Evaluate(GradientLastTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初期化処理
        ItemFlag = false;
        DrawFlag = false;
        OnGridMap = false;
        IsNoProblemRoute = true;
        currentPos = new(-1, -1, 0);

        // アイコンを非表示にする
        StartPosIcon.SetActive(false);
        EndPosIcon.SetActive(false);

        // LineRendererの初期設定
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
