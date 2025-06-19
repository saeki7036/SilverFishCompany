using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BeltDrawing : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    Gradient scsessGradient;

    [SerializeField]
    Gradient failedGradient;

    [SerializeField]
    GameObject BeltPrehab;

    [SerializeField]
    GameObject StratPosIcon;

    [SerializeField]
    SpriteRenderer StratIconSprite;

    [SerializeField]
    GameObject EndPosIcon;

    [SerializeField]
    SpriteRenderer EndIconSprite;

    const int ClampMin = 0;

    Vector2Int maxMapSize => GridMapManager.Instance.MaxMapSize;

    float GridAdjustScale => GridMapManager.Instance.GridAdjustScale();

    List<Vector3Int> SelectedPosList = new List<Vector3Int>();
    Dictionary<Vector3Int, int> posIndexMap = new Dictionary<Vector3Int, int>();

    // 左クリックした場所がグリッドマップ内かのフラグ
    bool OnGridMap;

    // 経路がすでにある建物と干渉していないか
    bool IsNoProblemRoute;

    // 触ったグリッドの場所
    Vector3Int currentPos;

    public void InputRegister(MouseController input)
    {
        input.LeftDownEvent += BeltDrawSetup;
        input.LeftClickEvent += DrawingBelt;
        input.LeftUpEvent += DrowBeltGenerate;
    }

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

    int ManhattanDistance2D(Vector3Int gridPos, Vector3Int targetPos)
    {
        int x = Mathf.Abs(targetPos.x - gridPos.x);
        int y = Mathf.Abs(targetPos.y - gridPos.y);

        return x + y;
    }

    bool IsInGridMap(Vector3 mouseWorldDownPos)
    {
        if(mouseWorldDownPos.x < -GridAdjustScale || 
            maxMapSize.x - GridAdjustScale < mouseWorldDownPos.x)
            return false;

        if(mouseWorldDownPos.y < -GridAdjustScale || 
            maxMapSize.y - GridAdjustScale < mouseWorldDownPos.y)
            return false;

        return true;
    }

    // 2つのグリッド座標を縦横の動きのみで対角線に近づけながら繋ぐ経路を生成する処理
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


    void BeltDrawSetup(Vector3 mouseWorldDownPos)
    {     
        SelectedPosList = new List<Vector3Int>();
        posIndexMap = new Dictionary<Vector3Int, int>();

        // ラインレンダラーを全消去（描画をリセット）
        lineRenderer.positionCount = 0;

        OnGridMap = IsInGridMap(mouseWorldDownPos);

        if (OnGridMap)
        {
            StratPosIcon.SetActive(true);
            EndPosIcon.SetActive(true);

            Vector3Int gridPos = GetMapGridInt(mouseWorldDownPos);

            StratPosIcon.transform.position = gridPos;
            EndPosIcon.transform.position = gridPos;

            posIndexMap.Add(gridPos, SelectedPosList.Count);
            SelectedPosList.Add(gridPos);
            currentPos = gridPos;
        } 
    }

    // ベルト（選択ライン）を描画する処理。マウスのワールド座標を元に選択ラインの更新を行う。
    void DrawingBelt(Vector3 mouseWorldPos)
    {
        if(!OnGridMap)     
            return;
        
        // マウスのワールド座標をグリッド座標（整数）に変換
        Vector3Int gridPos = GetMapGridInt(mouseWorldPos);

        // すでに何らかの位置が選択されてる場合に処理を実行
        if (SelectedPosList.Count != 0)
        {
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
            // 経路が問題ないか確認
            IsNoProblemRoute = RouteProblemCheck();

            // ラインレンダラーを更新して描画を反映
            UpdateLineRenderer();

            // 終点の位置にオブジェクトを移動する
            EndPosIcon.transform.position = gridPos;
        }   
    }

    void DrowBeltGenerate(Vector3 mouseWorldUpPos)
    {
        if (BeltPrehab == null)
            return;

        // LineRendererの描画を初期化
        lineRenderer.positionCount = 0;

        StratPosIcon.SetActive(false);
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

        List<GameObject> BeltList = new List<GameObject>();

        Vector3Int StratPos = SelectedPosList.First();
        Vector3Int EndPos = SelectedPosList.Last();

        //Debug.Log(StratPos + " : " + EndPos);

        // 終点から削除
        SelectedPosList.RemoveAt(SelectedPosList.Count - 1); // 終点を削除
        SelectedPosList.RemoveAt(0); // 先頭を削除

        foreach (Vector3Int posInt in SelectedPosList)
        {
            GameObject BeltObject = Instantiate(BeltPrehab,posInt,Quaternion.identity);
            BeltList.Add(BeltObject);
        }

        GridMapManager.Instance.BeltSetting(SelectedPosList, BeltList, StratPos, EndPos);
    }


    bool RouteProblemCheck()
    {
        // 先頭要素と終点要素を除いて既に建物があるか確認
        for (int i = 1; i < SelectedPosList.Count - 1; i++)
        {
            Vector2Int vector2Int = new Vector2Int()
            {
                x = SelectedPosList[i].x,
                y = SelectedPosList[i].y,
            };

            if (GridMapManager.Instance.GetCell(vector2Int).IsNoneCelltype())
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }


    // ラインレンダラーを更新して描画をする処理
    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = SelectedPosList.Count;

        // Vector3Int → Vector3 変換しつつ設定
        for (int i = 0; i < SelectedPosList.Count; i++)
        {
            // 高さ調整したい場合はZに+0.5fなどする
            lineRenderer.SetPosition(i, SelectedPosList[i]);
        }

        //経路の問題ないかで色変更
        GradientSetting();
    }
    void GradientSetting()
    {
        float GradientFirstTime = 0f;
        float GradientLastTime = 1f;

        Gradient gradient = IsNoProblemRoute ? scsessGradient : failedGradient;

        lineRenderer.colorGradient = gradient;
        StratIconSprite.color = gradient.Evaluate(GradientFirstTime);
        EndIconSprite.color = gradient.Evaluate(GradientLastTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnGridMap = false;
        IsNoProblemRoute = true;
        currentPos = new(-1, -1, 0);

        StratPosIcon.SetActive(false);
        EndPosIcon.SetActive(false);

        // LineRendererの初期設定（任意）
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
