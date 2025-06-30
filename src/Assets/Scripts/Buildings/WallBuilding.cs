using System.Collections.Generic;
using UnityEngine;

public class WallBuilding :GridBuilding
{
    public WallBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                      HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList)
                      : base(minBuildingPos, maxBuildingPos, importList, exportList)
    {
        Debug.Log("コンストラクタ：TurretBuilding");
        // ここで再度設定しない
    }

    public override void Operat()
    {
        return;
    }

    public override void ImportItem()
    {
        return;
    }

    public override void ExportItem()
    {
        return;
    }
}
