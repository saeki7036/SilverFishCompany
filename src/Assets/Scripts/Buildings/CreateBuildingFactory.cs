using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class CreateBuildingFactory
{
    // GridBuilding(抽象クラス)の派生クラスのインスタンスを宣言しているファクトリークラス

    /// <summary>
    /// 建物の種類に応じて対応したクラスを返す(因数の複雑化のため分割する予定)
    /// </summary>
    /// <param name="type">建物の種類</param>
    /// <param name="minBuldingPos">建物の左下の位置</param>
    /// <param name="maxBuldingPos">建物の右上の位置</param>
    /// <param name="importList">取り込み位置</param>
    /// <param name="exportList">排出位置</param>
    /// <param name="itemInfomation">アイテムの情報</param>
    /// <returns>種類に対応したクラス</returns>
    /// <exception cref="System.NotImplementedException">未実装の場合例外処理</exception>
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
