﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class BeltBuilding : GridBuilding
{
    BeltAnimation  beltAnimation;

    // コンストラクタ
    public BeltBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList)
                        : base(minBuildingPos, maxBuildingPos, importList, exportList)
    {
        Debug.Log("コンストラクタ：BeltBuilding");
        // ここで再度設定しない
    }

    public void SetBeltAnimation(BeltAnimation animation)=> beltAnimation = animation;

    public void BeltAnimPlay()
    {
        if (beltAnimation == null)
            return;

        beltAnimation.AnimationSetting(ImportPos, ExportPos);
    }

    public override void Operat()
    {
        // 呼び出しはBaceCamp側で行うのでreturn;
        return;
    }

    public override void ImportItem()
    {
        if (Item != null)
            return;

        List<Tuple<GridBuilding, Vector2Int>> possibleTupleList = new List<Tuple<GridBuilding, Vector2Int>>();

        foreach (Vector2Int import in ImportPos)
        {
            var importCell = GridMapManager.Instance.GetCell(import);

            BuildType importCellType = importCell.GridCellType;

            if (importCellType == BuildType.None || importCellType == BuildType.NULLTYPE)
                continue;

            GridBuilding importBuilding = importCell.Building;

            if (importBuilding == null)
                continue;

            if (importBuilding.IsEmptyItem() || importBuilding.Item.IsItemMove())
                continue;

            foreach (Vector2Int export in importBuilding.ExportPos)
            {
                var exportCell = GridMapManager.Instance.GetCell(export);

                BuildType exportCellType = importCell.GridCellType;

                if (exportCellType == BuildType.None || exportCellType == BuildType.NULLTYPE)
                    continue;

                GridBuilding exportBuilding = exportCell.Building;

                if (exportBuilding == null)
                    continue;

                if (this == exportBuilding)
                    possibleTupleList.Add(Tuple.Create(importBuilding, export));
            }
        }

        if (possibleTupleList.Count <= 0)
            return;

        var possibleTuple =  possibleTupleList[UnityEngine.Random.Range(0, possibleTupleList.Count)];

        Vector3 itemMovingPos = new() 
        { 
            x = possibleTuple.Item2.x, 
            y = possibleTuple.Item2.y, 
            z = 0 
        };

        Debug.Log(itemMovingPos);

        possibleTuple.Item1.Item.ItemMoveSetting(itemMovingPos);

        this.Item = possibleTuple.Item1.Item;

        possibleTuple.Item1.RemoveItem();
    }

    public override void ExportItem()
    {

    }
}
