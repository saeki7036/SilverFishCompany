using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class MultiBeltBuilding : GridBuilding
{
    // アイテムの集約と分配を行うベルトコンベアクラス

    enum BeltFunction
    {
        None,
        Division, //分配
        Aggregate, //集約
    }

    BeltFunction function;

    int CurrentExportPosValue;

    int CurrentExportIndexValue;

    // コンストラクタ
    public MultiBeltBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                             HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList)
                             : base(minBuildingPos, maxBuildingPos, importList, exportList)
    {
        if (importList.Count > 2)
            function = BeltFunction.Aggregate;

        else if (exportList.Count > 2)
            function = BeltFunction.Division;

        else
            function = BeltFunction.None;

        CurrentExportPosValue = 0;
        CurrentExportIndexValue = 0;
        Debug.Log("コンストラクタ：MultiBeltBuilding");
    }

    public override HashSet<Vector2Int> ExportPos
    {
        get => DivisionExport();
        set => base.ExportPos = value;
    }

    //HashSet<Vector2Int> returnExportPos = new();

    HashSet<Vector2Int> DivisionExport()
    {
        if (function != BeltFunction.Division)
            return base.ExportPos;

        // インポート可能な建物とエクスポート位置のタプル型リスト
        List<Tuple<GridBuilding, Vector2Int>> possibleTupleList = new List<Tuple<GridBuilding, Vector2Int>>();

        // すべてのエクスポート位置をチェック
        foreach (Vector2Int export in base.ExportPos)
        {
            var exportCell = GridMapManager.Instance.GetCell(export);

            BuildType exportCellType = exportCell.GridCellType;

            // セルが空の場合はスキップ
            if (exportCellType == BuildType.None || exportCellType == BuildType.NULLTYPE)
                continue;

            GridBuilding exportBuilding = exportCell.GetBuilding();

            if (exportBuilding == null)
                continue;

            // エクスポート建物のインポート先をチェック
            foreach (Vector2Int inport in exportBuilding.ImportPos)
            {
                var importCell = GridMapManager.Instance.GetCell(inport);

                BuildType importCellType = importCell.GridCellType;

                // セルが空の場合はスキップ
                if (importCellType == BuildType.None || importCellType == BuildType.NULLTYPE)
                    continue;

                GridBuilding importBuilding = importCell.GetBuilding();

                if (importBuilding == null)
                    continue;

                // インポート先がこの建物の場合、エクスポート位置を候補に追加
                if (this == importBuilding)
                {
                    possibleTupleList.Add(Tuple.Create(exportBuilding, export));
                    break;
                }
                    
            }
        }

        if (CurrentExportIndexValue != possibleTupleList.Count)
            CurrentExportPosValue = 0;

        CurrentExportIndexValue = possibleTupleList.Count;

        // エクスポート可能な候補がない場合は終了
        if (CurrentExportIndexValue <= 0)
            return base.ExportPos;

        // 候補から選択
        var possibleTuple = possibleTupleList[CurrentExportPosValue % CurrentExportIndexValue];

        Vector2Int TargetExportPos = possibleTuple.Item2;

        HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();

        hashSet.Add(TargetExportPos);

        return hashSet;
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

                BuildType exportCellType = exportCell.GridCellType;

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
        var possibleTuple = possibleTupleList[UnityEngine.Random.Range(0, possibleTupleList.Count)];

        // 移動先座標を3D座標に変換
        Vector3 itemMovingPos = new()
        {
            x = possibleTuple.Item2.x,
            y = possibleTuple.Item2.y,
            z = 0
        };

        // アイテムに移動設定を適用
        possibleTuple.Item1.Item.ItemMoveSetting(itemMovingPos);

        // このベルトにアイテムを設定
        this.Item = possibleTuple.Item1.Item;

        // 元の建物からアイテムクラスを削除
        possibleTuple.Item1.RemoveItem();

        // アイテム排出インデックスを変更
        CurrentExportPosValue = Math.Min(CurrentExportPosValue + 1, int.MaxValue);
    }

    public override void ExportItem()
    {
        return;
    }
}
