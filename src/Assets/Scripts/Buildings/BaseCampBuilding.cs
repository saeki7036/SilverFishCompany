using System;
using System.Collections.Generic;
using System.Linq;
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
        BFSImportSearch();
    }

    /// <summary>
    /// アイテム回収のための建物探索処理
    /// BFS（幅優先探索）を使って接続された建物チェーンからアイテムを回収
    /// </summary>
    void BFSImportSearch()
    {
        // 探索済み建物を記録するハッシュセット（重複探索防止）
        HashSet<GridBuilding> searchedHash = new HashSet<GridBuilding>();

        // BFS用のキュー
        Queue<GridBuilding> buildingQueue = new Queue<GridBuilding>();

        // 自信を探索済みとしてマーク
        searchedHash.Add(this);

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

        // 現在BFSを使用している
        // 探索後、はぐれ建物を順番関係なしに処理
        // 別途他の探索手法を試す必要あり

        // 建物一覧のDictionaryを取得
        var BuildingDictionary = GridMapManager.Instance.GetDictionary();

        // 探索用クラス
        BuildingSearch buildingSearch = new BuildingSearch(BuildingDictionary);

        // はぐれ建物のクラスを宣言
        GridBuilding SearchBuilding = null;
        // 候補なし => はぐれ建物を探索
        if (buildingQueue.Count == 0)
        {
            // 探索
            SearchBuilding = buildingSearch.SearchNext(searchedHash);

            // 候補があればQueueに追加
            if (SearchBuilding != null)
                buildingQueue.Enqueue(SearchBuilding);
            // なければreturnで処理終了させる
            else
                return;
        }          

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

            // 候補なし => はぐれ建物を探索
            if(buildingQueue.Count == 0)
            {
                // 探索
                SearchBuilding = buildingSearch.SearchNext(searchedHash);

                // 候補があればQueueに追加
                if (SearchBuilding != null)
                    buildingQueue.Enqueue(SearchBuilding);
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
        HashSet< Vector2Int > exportpos = new HashSet< Vector2Int >();


        // インポート建物のエクスポート先をすべてチェック
        foreach (Vector2Int pos in importBuilding.ExportPosBase)
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

        //Debug.Log(importBuilding + "のExport先に" + currentBuilding + "は無かった");

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

public class BuildingSearch
{
    private Dictionary<BuildType, HashSet<GridBuilding>> buildingDict;
    private List<BuildType> buildTypes;

    // 現在の探索位置を記録
    private int currentTypeIndex = 0;
    private int currentBuildingIndex = 0;

    // コンストラクタ
    public BuildingSearch(Dictionary<BuildType, HashSet<GridBuilding>> dict)
    {
        buildingDict = dict;
        buildTypes = buildingDict.Keys.ToList();
        Reset();
    }

    /// <summary>
    /// 特定条件を満たす建物を探索する。条件を満たした時点でbreak。
    /// 次回呼び出し時は続きから探索を再開する。
    /// </summary>
    public GridBuilding? SearchNext(HashSet<GridBuilding> searchedHash)
    {
        for (; currentTypeIndex < buildTypes.Count; currentTypeIndex++)
        {
            BuildType type = buildTypes[currentTypeIndex];
            var buildings = buildingDict[type].ToList();

            for (; currentBuildingIndex < buildings.Count; currentBuildingIndex++)
            {
                var building = buildings[currentBuildingIndex];

                // 探索済みHashSetに含まれてなければ値を返す
                if (!searchedHash.Contains(building))
                {
                    // 次回はこの次から探索を再開
                    currentBuildingIndex++;
                    return building; // 条件一致で停止
                }
            }

            // 現在のBuildType内をすべて探索し終えたら、次のタイプへ
            currentBuildingIndex = 0;
        }

        // すべて探索済み
        Reset();
        return null;
    }

    /// <summary>
    /// 探索位置をリセット
    /// </summary>
    public void Reset()
    {
        currentTypeIndex = 0;
        currentBuildingIndex = 0;
    }
}
