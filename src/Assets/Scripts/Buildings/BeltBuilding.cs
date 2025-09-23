using System;
using System.Collections.Generic;
using UnityEngine;

public class BeltBuilding : GridBuilding
{
    // アイテムを運ぶベルトコンベアクラス

    BeltAnimation  beltAnimation;

    // コンストラクタ
    public BeltBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList)
                        : base(minBuildingPos, maxBuildingPos, importList, exportList)
    {
        Debug.Log("コンストラクタ：BeltBuilding");
        // ここで再度設定しない
    }


    //複数の分岐を持たせる処理は未実装のため要実装


    /// <summary>
    /// ベルトアニメーションコンポーネントを設定
    /// コンストラクタに混ぜたほうが誤設定が無くなるので変更予定
    /// </summary>
    /// <param name="animation">設定するベルトアニメーション</param>
    public void SetBeltAnimation(BeltAnimation animation)=> beltAnimation = animation;

    /// <summary>
    /// ベルトアニメーションを再生開始
    /// インポート・エクスポート位置情報を使ってアニメーション設定
    /// </summary>
    public void BeltAnimPlay()
    {
        if (beltAnimation == null)
            return;

        beltAnimation.AnimationSetting(ImportPos, ExportPos);
    }

    /// <summary>
    /// 建物の動作処理
    /// ベルトは他の建物によって制御される.
    /// BaseCamp側で呼び出し制御されるため、ここでは処理なし
    /// </summary>
    public override void Operat()
    {
        // 呼び出しはBaceCamp側で行うのでreturn;
        return;
    }

    /// <summary>
    /// アイテムのインポート処理 
    /// 接続建物からアイテムを受け取る
    /// 複数の候補がある場合はランダムに選択してアイテムを移動
    /// </summary>
    public override void ImportItem()
    {
        // 既にアイテムを持っている場合は処理しない
        if (Item != null)
            return;

        // インポート可能な建物とエクスポート位置のタプル型リスト
        List<Tuple<GridBuilding, Vector2Int>> possibleTupleList = new List<Tuple<GridBuilding, Vector2Int>>();

        // すべてのインポート位置をチェック
        foreach (Vector2Int import in ImportPos)
        {
            var importCell = GridMapManager.Instance.GetCell(import);

            BuildType importCellType = importCell.GridCellType;

            // セルが空の場合はスキップ
            if (importCellType == BuildType.None || importCellType == BuildType.NULLTYPE)
                continue;

            GridBuilding importBuilding = importCell.GetBuilding();

            if (importBuilding == null)
                continue;

            // アイテムが存在しない、または移動中の場合はスキップ
            if (importBuilding.IsEmptyItem() || importBuilding.Item.IsItemMove())
                continue;

            // インポート建物のエクスポート先をチェック
            foreach (Vector2Int export in importBuilding.ExportPos)
            {
                var exportCell = GridMapManager.Instance.GetCell(export);

                BuildType exportCellType = importCell.GridCellType;

                // セルが空の場合はスキップ
                if (exportCellType == BuildType.None || exportCellType == BuildType.NULLTYPE)
                    continue;

                GridBuilding exportBuilding = exportCell.GetBuilding();

                if (exportBuilding == null)
                    continue;

                // エクスポート先がこの建物の場合、候補に追加
                if (this == exportBuilding)
                    possibleTupleList.Add(Tuple.Create(importBuilding, export));
            }
        }

        // インポート可能な候補がない場合は終了
        if (possibleTupleList.Count <= 0)
            return;

        // 複数候補からランダムに選択
        var possibleTuple =  possibleTupleList[UnityEngine.Random.Range(0, possibleTupleList.Count)];

        // 移動先座標を3D座標に変換
        Vector3 itemMovingPos = new() 
        { 
            x = possibleTuple.Item2.x, 
            y = possibleTuple.Item2.y, 
            z = 0 
        };

        Debug.Log(itemMovingPos);

        // アイテムに移動設定を適用
        possibleTuple.Item1.Item.ItemMoveSetting(itemMovingPos);

        // このベルトにアイテムを設定
        this.Item = possibleTuple.Item1.Item;

        // 元の建物からアイテムクラスを削除
        possibleTuple.Item1.RemoveItem();
    }

    public override void ExportItem()
    {
        return;
    }
}
