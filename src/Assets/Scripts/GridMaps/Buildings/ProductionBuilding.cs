using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionBuilding : GridBuilding
{
    ItemInformation itemInfo;

    // コンストラクタ
    public ProductionBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList,
                        ItemInformation itemInfomation)
                        : base(minBuildingPos, maxBuildingPos, importList, exportList)
    {    
        // ここで抽象クラス側にない変数のみ設定
        itemInfo = itemInfomation;

        Debug.Log("コンストラクタ：ProductionBuilding");

        // 生産施設の生産位置は現在1か所のみの想定
        if (ImportPos.Count == 0)
        {
            Debug.LogAssertion("生産位置が登録されていません");
        }
        else if (ImportPos.Count > 1)
        {
            Debug.LogAssertion("生産位置が複数登録されています");
        }
    }

    public override void Operat()
    {
        if (ImportPos.Count != 1)
        {
            return;
        }

        if (Item == null)
        {
            var instance = ItemManager.Instance;

            Item = new ProductItem(itemInfo, ImportPos.First(), instance.GetMaxTimeCount());

            instance.AddListItem(Item);
        } 
    }

    public override void ImportItem()
    {
        // アイテムの取り込みは行わないのでreturn。
        return;
    }

    public override void ExportItem()
    {

    }
}
