using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BaseCampBuilding : GridBuilding
{
    List<ProductItem> productItems;

    // Import先の建物がExport先の位置を持っていない時のパラメータ
    readonly Vector3 NoTargetPos = -Vector3.one;

    // コンストラクタ
    public BaseCampBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos, 
                            HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList) 
                            : base(minBuildingPos, maxBuildingPos,importList, exportList)
    {
        
        // ここで抽象クラスで設定したものは再度設定しない
        productItems = new List<ProductItem>();

        Debug.Log("コンストラクタ：BaseCampBuilding");
    }

    public override void Operat()
    {
        // Removeを扱うので逆順ループ
        for (int i = productItems.Count - 1; i >= 0; i--)
        {
            if (!productItems[i].IsItemMove())
            {
                productItems[i].ItemObjectDestroy();  // 削除前に呼びたい処理

                ItemStock(productItems[i]);

                productItems.RemoveAt(i);             // 削除
            }
        }
    }

    public override void ImportItem()
    {
        ImportSearch();
    }

    public override void ExportItem()
    {
        // アイテムの排出は行わないのでreturn。
        return;
    }

    void ImportSearch()
    {
        HashSet<GridBuilding> searchedHash = new HashSet<GridBuilding>();

        Queue<GridBuilding> buildingQueue = new Queue<GridBuilding>();

        foreach (Vector2Int pos in ImportPos)
        {
            GridBuilding gridBuilding = GetValidBuilding(pos);

            if (gridBuilding == null)
                continue;

            Vector3 TargetPos = GetMoveTargetPos(this,gridBuilding);

            if (TargetPos == NoTargetPos)
                continue;

            buildingQueue.Enqueue(gridBuilding);

            ItemRecovery(TargetPos, gridBuilding);
        }

        while (buildingQueue.Count > 0)
        {
            GridBuilding currntBuilding = buildingQueue.Dequeue();

            searchedHash.Add(currntBuilding);

            currntBuilding.ImportItem();

            foreach (Vector2Int pos in currntBuilding.ImportPos)
            {
                GridBuilding gridBuilding = GetValidBuilding(pos);

                if (gridBuilding == null)
                    continue;

                Vector3 TargetPos = GetMoveTargetPos(currntBuilding,gridBuilding);

                if (TargetPos == NoTargetPos)
                    continue;

                if(!searchedHash.Contains(gridBuilding))
                    buildingQueue.Enqueue(gridBuilding);
            }
        }
    }

    void ItemRecovery(Vector3 targetPos, GridBuilding gridBuilding)
    {
        if (gridBuilding.Item == null || gridBuilding.Item.IsItemMove())
            return;

        gridBuilding.Item.ItemMoveSetting(targetPos);

        productItems.Add(gridBuilding.Item);

        gridBuilding.RemoveItem();
    }

    Vector3 GetMoveTargetPos(GridBuilding currentBuilding, GridBuilding importBuilding)
    {
        foreach(Vector2Int pos in importBuilding.ExportPos)
        {
            GridBuilding exportBuilding = GetValidBuilding(pos);

            //CellType cellType = GridMapManager.Instance.GetCell(pos).GridCellType;
            //Debug.Log(cellType + ":" + pos);

            //if(cellType == CellType.None || cellType == CellType.NULLTYPE)
            if (exportBuilding == null ||  currentBuilding != exportBuilding)
            {
                continue;
            }

            Vector2 targetPos = pos;
            return targetPos;
        }

        Debug.Log(importBuilding + "のExport先に" + currentBuilding + "は無かった");

        return NoTargetPos;
    }


    void ItemStock(ProductItem productItem)
    {
        Debug.Log("アイテムカデゴリ：" + productItem.GetCategory() + 
                  "アイテムレベル："+ productItem.GetLevel().ToString());
    }
}
