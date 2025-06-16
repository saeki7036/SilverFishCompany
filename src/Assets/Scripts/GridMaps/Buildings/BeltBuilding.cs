using System.Collections.Generic;
using UnityEngine;

public class BeltBuilding : GridBuilding
{
    // コンストラクタ
    public BeltBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList)
                        : base(minBuildingPos, maxBuildingPos, importList, exportList)
    {
        Debug.Log("コンストラクタ：BeltBuilding");
        // ここで再度設定しない
    }

    public override void OperatFacility()
    {
        // 呼び出しはBaceCamp側で行うのでreturn;
        return;
    }

    public override void ImportItem()
    {

    }

    public override void ExportItem()
    {

    }
}
