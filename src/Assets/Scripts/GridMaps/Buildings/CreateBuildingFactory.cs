using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class CreateBuildingFactory
{
    
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
            Debug.Log("None‘ã“ü");
            throw new ArgumentException($"Unknown building type: {type}");
            //return null;
            */
        };
    }
}
