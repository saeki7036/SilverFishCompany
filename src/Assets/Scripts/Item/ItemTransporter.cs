using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムの輸送を管理するクラス
/// アイテムプールを保持し、各アイテムの移動処理を制御
/// </summary>
public class ItemTransporter
{
    List<ProductItem> itemPool;
    float addTimeCount;

    // コンストラクタ
    /// <param name="addTimeCount">アイテム移動の時間カウント</param>
    public ItemTransporter(float addTimeCount)
    {
        itemPool = new List<ProductItem>();
        this.addTimeCount = addTimeCount;
    }

    /// <summary>
    /// アイテムをプールに追加
    /// </summary>
    /// <param name="item">追加するアイテム</param>
    public void AddPool(ProductItem item) => itemPool.Add(item);

    /// <summary>
    /// アイテムプール内の全アイテムの移動チェックと処理
    /// 無効なアイテムの削除も行う
    /// </summary>
    public void ItemMovingCheck()
    {
        // Removeを扱うので逆順ループ
        for (int i = itemPool.Count - 1; i >= 0; i--)
        {
            // nullチェック
            if (itemPool[i] == null)
            {
                itemPool.RemoveAt(i);
                continue;
            }

            // オブジェクトがなければ削除する
            if (itemPool[i].IsEnptyItemObject())
            {
                itemPool.RemoveAt(i);
                continue;
            }

            // アイテムが移動中の場合、移動処理を実行
            if (itemPool[i].IsItemMove())
            {
                itemPool[i].ItemMovement(addTimeCount);  // アイテム運搬処理
            }
        }
    }
}
