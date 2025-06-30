using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProcessingBuilding : GridBuilding
{
    ItemInformation itemInfo;

    // コンストラクタ
    public ProcessingBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList,
                        ItemInformation itemInfomation)
                        : base(minBuildingPos, maxBuildingPos, importList, exportList)
    {
        // ここで抽象クラス側にない変数のみ設定
        itemInfo = itemInfomation;

        Debug.Log("コンストラクタ：ProcessingBuilding");

        // 加工施設は現在1か所のみずつの想定
        if (ImportPos.Count == 0)
        {
            Debug.LogAssertion("実行位置が登録されていません");
        }
        else if (ImportPos.Count > 1)
        {
            Debug.LogAssertion("実行位置が複数登録されています");
        }
        if (ExportPos.Count == 0)
        {
            Debug.LogAssertion("排出位置が登録されていません");
        }
        else if (ExportPos.Count > 1)
        {
            Debug.LogAssertion("排出位置が複数登録されています");
        }
    }

    public override void Operat()
    {
        if (ImportPos.Count != 1 || ExportPos.Count != 1)
        {
            return;
        }

        if (Item != null && Item.IsSetNextLevelInfo())
        {
            if(Item.IsItemUpdate())
                Item.NextLevelSetting();
        }
    }

    public override void ImportItem()
    {
        if (Item != null)
            return;

        List<Tuple<GridBuilding, Vector2Int>> possibleTupleList = new List<Tuple<GridBuilding, Vector2Int>>();

        Vector2Int importPosFirst = ImportPos.First();

        GridBuilding gridBuilding = GetValidBuilding(importPosFirst);

        if (gridBuilding == null)
            return;

        if (gridBuilding.IsEmptyItem() || gridBuilding.Item.IsItemMove() 
            || gridBuilding.Item.IsSameCategoryAndNearLevel(itemInfo))
            return;
        
        foreach (Vector2Int export in gridBuilding.ExportPos)
        {
            GridBuilding exportBuilding = GetValidBuilding(export);
            
            if (exportBuilding == null)
                continue;

            if (this == exportBuilding)
                possibleTupleList.Add(Tuple.Create(gridBuilding, export));
        }

        if (possibleTupleList.Count <= 0)
            return;
        
        var possibleTuple = possibleTupleList.First();

        Vector3 itemMovingPos = new()
        {
            x = possibleTuple.Item2.x,
            y = possibleTuple.Item2.y,
            z = 0
        };

        possibleTuple.Item1.Item.ItemMoveSetting(itemMovingPos);

        this.Item = possibleTuple.Item1.Item;

        possibleTuple.Item1.RemoveItem();

        Item.SetNextLevelInfo(itemInfo);
    }

    public override void ExportItem()
    {

    }
}
