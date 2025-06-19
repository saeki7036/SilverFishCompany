using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class CreateBuildingFactory
{
    // GridBuilding(抽象クラス)の派生クラスのインスタンスを宣言しているファクトリークラス
    public static GridBuilding CreateBuilding(CellType type, 
        Vector2Int minBuldingPos, Vector2Int maxBuldingPos,
        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList,
        ItemInformation itemInfomation = null)
    {
        return type switch
        {
            CellType.Belt => new BeltBuilding(minBuldingPos, maxBuldingPos, importList, exportList),
            CellType.BaseCamp => new BaseCampBuilding(minBuldingPos, maxBuldingPos, importList, exportList),
            CellType.Production => new ProductionBuilding(minBuldingPos, maxBuldingPos, importList, exportList,itemInfomation),
            //CellType.Factory => new FactoryBuilding(position, name),
            //CellType.Tower => new TowerBuilding(position, height),
            _ => throw new System.NotImplementedException()

            /*
            Debug.Log("None代入");
            throw new ArgumentException($"Unknown building type: {type}");
            //return null;
            */
        };
    }
}
