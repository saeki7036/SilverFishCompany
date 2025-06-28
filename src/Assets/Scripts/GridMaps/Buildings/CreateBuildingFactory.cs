using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class CreateBuildingFactory
{
    // GridBuilding(抽象クラス)の派生クラスのインスタンスを宣言しているファクトリークラス
    public static GridBuilding CreateBuilding(BuildType type, 
        Vector2Int minBuldingPos, Vector2Int maxBuldingPos,
        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList,
        ItemInformation itemInfomation = null)
    {
        return type switch
        {
            BuildType.Belt => new BeltBuilding(minBuldingPos, maxBuldingPos, importList, exportList),
            BuildType.BaseCamp => new BaseCampBuilding(minBuldingPos, maxBuldingPos, importList, exportList),
            BuildType.Production => new ProductionBuilding(minBuldingPos, maxBuldingPos, importList, exportList,itemInfomation),
            BuildType.Processing => new ProcessingBuilding(minBuldingPos, maxBuldingPos, importList, exportList, itemInfomation),

            BuildType.Turret => new TurretBuilding(minBuldingPos, maxBuldingPos, importList, exportList),
            BuildType.Wall => new WallBuilding(minBuldingPos, maxBuldingPos, importList, exportList),

            _ => throw new System.NotImplementedException()

            /*
            Debug.Log("None代入");
            throw new ArgumentException($"Unknown building type: {type}");
            //return null;
            */
        };
    }
}
