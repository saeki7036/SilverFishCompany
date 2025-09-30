using System.Collections.Generic;
using UnityEngine;

public class BaseCampBuilding : GridBuilding
{
    // アイテムの回収・パラメータ変換を行う建物クラス

    List<ProductItem> productItems;// 回収したアイテムを一時的に格納するリスト

    // Import先の建物がExport先の位置を持っていない時のパラメータ
    static readonly Vector3 NoTargetPos = -Vector3.one;

    // コンストラクタ
    public BaseCampBuilding(Vector2Int minBuildingPos, Vector2Int maxBuildingPos, 
                            HashSet<Vector2Int> importList, HashSet<Vector2Int> exportList) 
                            : base(minBuildingPos, maxBuildingPos,importList, exportList)
    {
        
        // ここで抽象クラスで設定したものは再度設定しない
        productItems = new List<ProductItem>();

        Debug.Log("コンストラクタ：BaseCampBuilding");
    }

    /// <summary>
    /// 建物の動作処理 - 移動完了したアイテムの保管処理
    /// 移動が完了したアイテムをパラメータに変換し、リストから削除する
    /// </summary>
    public override void Operat()
    {
        // Removeを扱うので逆順ループ
        for (int i = productItems.Count - 1; i >= 0; i--)
        {
            // アイテムの移動が完了しているかチェック
            if (!productItems[i].IsItemMove())
            {
                // オブジェクト削除
                productItems[i].ItemObjectDestroy();  

                // アイテムを保管
                ItemStock(productItems[i]);

                // クラス削除
                productItems.RemoveAt(i); 
            }
        }
    }

    /// <summary>
    /// アイテムのインポート処理 - 他の建物からアイテムを回収
    /// </summary>
    public override void ImportItem()
    {
        ImportSearch();
    }

    /// <summary>
    /// アイテム回収のための建物探索処理
    /// BFS（幅優先探索）を使って接続された建物チェーンからアイテムを回収
    /// </summary>
    void ImportSearch()
    {
        // 探索済み建物を記録するハッシュセット（重複探索防止）
        HashSet<GridBuilding> searchedHash = new HashSet<GridBuilding>();

        // BFS用のキュー
        Queue<GridBuilding> buildingQueue = new Queue<GridBuilding>();

        // 直接接続された建物からアイテム回収を開始
        foreach (Vector2Int pos in ImportPos)
        {
            GridBuilding gridBuilding = GetValidBuilding(pos);

            if (gridBuilding == null)
                continue;

            // 移動先座標を取得（この建物への移動が可能かチェック）
            Vector3 TargetPos = GetMoveTargetPos(this,gridBuilding);

            if (TargetPos == NoTargetPos)
                continue;

            // 探索キューに追加
            buildingQueue.Enqueue(gridBuilding);

            // アイテム回収実行
            ItemRecovery(TargetPos, gridBuilding);
        }

        // 現在BFSを使用しているが、接続していない建物のアイテム移動が行われないため
        // 別途他の探索手法を試す必要あり

        // BFSでチェーン接続された全建物を探索
        while (buildingQueue.Count > 0)
        {
            // Queueから取り出し
            GridBuilding currntBuilding = buildingQueue.Dequeue();

            // 探索済みとしてマーク
            searchedHash.Add(currntBuilding);

            // 現在の建物のインポート処理を実行
            currntBuilding.ImportItem();

            // 現在の建物に接続された建物をさらに探索
            foreach (Vector2Int pos in currntBuilding.ImportPos)
            {
                GridBuilding gridBuilding = GetValidBuilding(pos);

                if (gridBuilding == null)
                    continue;

                // 移動先座標を取得
                Vector3 TargetPos = GetMoveTargetPos(currntBuilding,gridBuilding);

                if (TargetPos == NoTargetPos)
                    continue;

                // 未探索の建物のみキューに追加
                if (!searchedHash.Contains(gridBuilding))
                    buildingQueue.Enqueue(gridBuilding);
            }
        }
    }

    /// <summary>
    /// 指定建物からアイテムを回収し、移動設定を行う
    /// </summary>
    /// <param name="targetPos">移動先座標</param>
    /// <param name="gridBuilding">回収元の建物</param>
    void ItemRecovery(Vector3 targetPos, GridBuilding gridBuilding)
    {
        // アイテムが存在し、移動中でないことを確認
        if (gridBuilding.Item == null || gridBuilding.Item.IsItemMove())
            return;

        // アイテムに移動先を設定
        gridBuilding.Item.ItemMoveSetting(targetPos);

        // 回収リストに追加
        productItems.Add(gridBuilding.Item);

        // 元の建物からアイテムを削除
        gridBuilding.RemoveItem();
    }

    /// <summary>
    /// 移動先座標を取得する処理
    /// インポート建物のエクスポート先に現在の建物が含まれているかチェック
    /// </summary>
    /// <param name="currentBuilding">現在の建物（移動先）</param>
    /// <param name="importBuilding">インポート元の建物</param>
    /// <returns>移動先座標、見つからない場合はNoTargetPosを返す</returns>
    Vector3 GetMoveTargetPos(GridBuilding currentBuilding, GridBuilding importBuilding)
    {
        // インポート建物のエクスポート先をすべてチェック
        foreach (Vector2Int pos in importBuilding.ExportPos)
        {
            GridBuilding exportBuilding = GetValidBuilding(pos);

            //CellType cellType = GridMapManager.Instance.GetCell(pos).GridCellType;
            //Debug.Log(cellType + ":" + pos);
            //if(cellType == CellType.None || cellType == CellType.NULLTYPE)

            // エクスポート先が現在の建物と一致するかチェック
            if (exportBuilding == null ||  currentBuilding != exportBuilding)
            {
                continue;
            }

            // 一致した座標を返す
            Vector2 targetPos = pos;

            // Debug.Log("一致した位置" + targetPos);
            return targetPos;
        }

        Debug.Log(importBuilding + "のExport先に" + currentBuilding + "は無かった");

        return NoTargetPos;
    }


    /// <summary>
    /// アイテムを保管する処理
    /// ItemManagerにアイテムのカテゴリとレベル情報を渡して保管
    /// </summary>
    /// <param name="productItem">保管するアイテム</param>
    void ItemStock(ProductItem productItem)
    {
        ItemManager.Instance.AddItemStorage(productItem.GetCategory(), productItem.GetLevel());
        //Debug.Log("アイテムカデゴリ：" + productItem.GetCategory() + 
        //          "アイテムレベル："+ productItem.GetLevel().ToString());
    }

    public override void ExportItem()
    {
        // アイテムの排出は行わないのでreturn。
        return;
    }

    //以下、Enemy探索用関数
    //=============================================================
    public HashSet<Vector2Int> GetVectorIntGridPos()
    {
        HashSet<Vector2Int> BaseCampPos = new HashSet<Vector2Int>();

        for(int x = MinBuildingPos.x; x <= MaxBuildingPos.x; x++)
        {
            for (int y = MinBuildingPos.y; y <= MaxBuildingPos.y; y++)
            {
                BaseCampPos.Add(new Vector2Int(x, y));
            }
        }

        return BaseCampPos;
    }

}
