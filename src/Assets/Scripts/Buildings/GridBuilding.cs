using UnityEngine;
using System.Collections.Generic;

public abstract class GridBuilding
{
    // 建物の抽象クラス

    HashSet<Vector2Int> importPos;// 作業の実行位置
    HashSet<Vector2Int> exportPos;// 排出位置

    // gridmapのオブジェクトの空間位置
    Vector2Int minBuildingPos;
    Vector2Int maxBuildingPos;

    ProductItem item; // 建物が持つアイテム

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

    /// <summary>
    /// アイテムが空かどうかを判定
    /// </summary>
    /// <returns>true: アイテムが空, false: アイテムを保持</returns>
    public bool IsEmptyItem() => item == null;

    /// <summary>
    /// 保持アイテムを削除（nullに設定）
    /// </summary>
    public void RemoveItem()=> item = null;

    // コンストラクタ

    /// <summary>
    /// GridBuildingのコンストラクタ
    /// 建物の座標範囲と取り込み・排出位置を初期化
    /// </summary>
    /// <param name="minBuildingPos">建物の最小座標</param>
    /// <param name="maxBuildingPos">建物の最大座標</param>
    /// <param name="importList">取り込み位置のリスト</param>
    /// <param name="exportList">排出位置のリスト</param>
    public GridBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos,
                        HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList)
    {
        this.minBuildingPos = minBuildingPos;
        this.maxBuildingPos = maxBuildingPos;
        this.importPos = new HashSet<Vector2Int>(importList);
        this.exportPos = new HashSet<Vector2Int>(exportList);

        // アイテムを初期化（空の状態に設定）
        RemoveItem();

        //Debug.Log("コンストラクタ：抽象");
    }


    /// <summary>
    /// 指定位置の有効な建物を取得
    /// BuildType.NoneやNULLTYPEの場合はnullを返す
    /// </summary>
    /// <param name="pos">取得したい建物の座標</param>
    /// <returns>有効な建物インスタンス、または無効な場合はnull</returns>
    protected GridBuilding GetValidBuilding(Vector2Int pos)
    {
        var cell = GridMapManager.Instance.GetCell(pos);
        var cellType = cell.GridCellType;

        if (cellType == BuildType.None || cellType == BuildType.NULLTYPE) 
            return null;

        return cell.GetBuilding();
    }

    /// <summary>
    /// 建物特有の動作を行う処理
    /// </summary>
    public abstract void Operat();

    /// <summary>
    /// 他の建物からアイテムを取り込む処理
    /// </summary>
    public abstract void ImportItem();

    /// <summary>
    /// アイテムを排出する機能
    /// そのうちアイテムを取得できるかのboolメゾットに変更予定
    /// </summary>
    public abstract void ExportItem();
}
