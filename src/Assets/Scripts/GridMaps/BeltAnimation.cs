using System.Collections.Generic;
using UnityEngine;

public class BeltAnimation : MonoBehaviour
{
    [SerializeField] Animator beltAnimator;// ベルトアニメーション用のAnimator

    [SerializeField] Transform beltTransform;// ベルトの Transform(回転制御)

    [SerializeField] BeltAnimConfig beltAnimConfig;// ベルトアニメーション設定データ

    /// <summary>
    /// 4方向の隣接セルをチェックするためのオフセット配列
    /// 順序: 上(0,1), 下(0,-1), 左(-1,0), 右(1,0)
    /// </summary>
    static readonly Vector2Int[] DirectionOffset = new Vector2Int[4]
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };

    /// <summary>
    /// ベルトの現在位置をVector2Intで取得
    /// Transform座標を四捨五入してグリッド座標に変換
    /// </summary>
    /// <returns>グリッド座標でのベルト位置</returns>
    Vector2Int posVec2Int() => new Vector2Int()
    {
        x = Mathf.RoundToInt(beltTransform.position.x),
        y = Mathf.RoundToInt(beltTransform.position.y)
    };

    /// <summary>
    /// 4桁のインデックス文字列から適切な回転角度を計算
    /// 各桁は上下左右の接続状況を表し、'1'の位置に応じて回転角度を決定
    /// </summary>
    /// <param name="index">4桁の接続状況文字列（上下左右の順）</param>
    /// <returns>Z軸回転のQuaternion</returns>
    Quaternion GetQuaternionZ(string index)
    {
        int rotateValue = 0;

        if(index.Length != 4)
        {
            Debug.LogAssertion("範囲外のサイズ:"+ index.Length);
            return Quaternion.identity;
        }

        if (index[0] == '1')
            rotateValue = 180;
        if (index[1] == '1')
            rotateValue = 0;
        if (index[2] == '1')
            rotateValue = 270;
        if (index[3] == '1')
            rotateValue = 90;

        return Quaternion.Euler(0, 0, rotateValue);
    }

    void Start()
    {
        // 初期化処理

        // ベルトアニメーション設定を初期化
        beltAnimConfig.Initialize();

        // 自身の位置にあるグリッドセルを取得
        var beltCell = GridMapManager.Instance.GetCell(posVec2Int());

        // ベルトタイプのセルの場合のみ処理
        if (beltCell.GridCellType == BuildType.Belt)
        {
            // キャスト成功時のみ実行
            if (beltCell.GetBuilding() is BeltBuilding beltBuilding)
            {
                // BeltBuildingにこのアニメーションコンポーネントを登録
                beltBuilding.SetBeltAnimation(this);
                // ベルトアニメーションを開始
                beltBuilding.BeltAnimPlay();
            }
        }
    }

    /// <summary>
    /// ベルトアニメーションの設定処理
    /// 取り込み・排出位置の情報から隣接状況を判定し、適切なアニメーションと回転を設定
    /// </summary>
    /// <param name="inport">アイテム取り込み位置のセット</param>
    /// <param name="export">アイテム排出位置のセット</param>
    public void AnimationSetting(HashSet<Vector2Int> inport, HashSet<Vector2Int> export)
    {
        Vector2Int beltPosVec2Int = posVec2Int();

        string index = "";

        // 4方向の隣接セルをチェックして接続状況を文字列化
        foreach (Vector2Int direction in DirectionOffset)
        {
            Vector2Int neighbor = beltPosVec2Int + direction;

            int value = 0;

            // 取り込み位置にある場合は1
            if (inport.Contains(neighbor)) 
                value = 1;

            // 排出位置にある場合は2
            if (export.Contains(neighbor)) 
                value = 2;

            index += value.ToString();
        }

        // 接続パターンからアニメーションタイプを取得
        AnimType type = beltAnimConfig.GetAnimType(index);

        // アニメーションタイプからアニメーション名を取得
        string animName = beltAnimConfig.GetAnimName(type);

        // 取り込み・排出位置が存在しない、またはアニメーションタイプが無効な場合は処理終了
        if (inport.Count == 0 || export.Count == 0 || type == AnimType.None)
            return;

        // 複数方向の場合が未定義なので用実装

        // 単一方向の接続の場合は回転を設定
        if (inport.Count == 1 || export.Count == 1)
        {
            beltTransform.rotation = GetQuaternionZ(index);
        }

        // アニメーション再生
        beltAnimator.Play(animName);
    }
}
