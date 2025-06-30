using UnityEngine;
using System.Collections.Generic;

public abstract class GridBuilding
{
    HashSet<Vector2Int> importPos;// 作業の実行位置
    HashSet<Vector2Int> exportPos;// 排出位置

    //削除する際にgridmap側の情報を削除する際に必要な位置のキャッシュ
    Vector2Int minBuildingPos;
    Vector2Int maxBuildingPos;

    ProductItem item;

    // プロパティ
    public HashSet<Vector2Int> ImportPos
    {
        get => importPos;
        set => importPos = value;
    }
    public HashSet<Vector2Int> ExportPos
    {
        get => exportPos;
        set => exportPos = value;
    }
    public Vector2Int MinBuildingPos
    {
        get => minBuildingPos;
        set => minBuildingPos = value;
    }
    public Vector2Int MaxBuildingPos
    {
        get => maxBuildingPos;
        set => maxBuildingPos = value;
    }

    public ProductItem Item
    {
        get => item;
        set => item = value;
    }

    public bool IsEmptyItem() => item == null;

    public void RemoveItem()=> item = null;

    // コンストラクタ
    public GridBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList)
    {
        this.minBuildingPos = minBuildingPos;
        this.maxBuildingPos = maxBuildingPos;
        this.importPos = new HashSet<Vector2Int>(importList);
        this.exportPos = new HashSet<Vector2Int>(exportList);

        RemoveItem();

        //Debug.Log("コンストラクタ：抽象");
    }

    protected GridBuilding GetValidBuilding(Vector2Int pos)
    {
        var cell = GridMapManager.Instance.GetCell(pos);
        var cellType = cell.GridCellType;

        if (cellType == BuildType.None || cellType == BuildType.NULLTYPE) return null;
        return cell.Building;
    }

    public abstract void Operat();

    public abstract void ImportItem();

    public abstract void ExportItem();
}
