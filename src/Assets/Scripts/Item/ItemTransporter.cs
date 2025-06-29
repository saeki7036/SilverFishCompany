using System.Collections.Generic;
using UnityEngine;

public class ItemTransporter
{
    List<ProductItem> ItemPool;
    float AddTimeCount;

    public ItemTransporter(float addTimeCount)
    {
        ItemPool = new List<ProductItem>();
        AddTimeCount = addTimeCount;
    }

    public void AddPool(ProductItem item) => ItemPool.Add(item);


    public void ItemMovingCheck()
    {
        // Removeを扱うので逆順ループ
        for (int i = ItemPool.Count - 1; i >= 0; i--)
        {
            // nullチェック
            if (ItemPool[i] == null)
            {
                ItemPool.RemoveAt(i);
                continue;
            }

            // オブジェクトがなければ削除する
            if (ItemPool[i].IsEnptyItemObject())
            {
                ItemPool.RemoveAt(i);
                continue;
            }

            // アイテム運搬処理
            if (ItemPool[i].IsItemMove())
            {
                ItemPool[i].ItemMovement(AddTimeCount);
            }
        }
    }
}
