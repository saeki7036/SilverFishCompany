using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保有しているアイテムを管理するクラス
/// 各アイテムの取得、消費、表示を制御
/// </summary>
public class ItemStocker
{
    Dictionary<ItemCategory, Dictionary<int, int>> itemStorage;

    // コンストラクタ
    public ItemStocker(ItemConfig itemConfig)
    {
        // 初期化
        itemStorage = new Dictionary<ItemCategory, Dictionary<int, int>>();

        // List内を初期化
        foreach (var useItem in itemConfig.GetUseItemList())
        {
            ItemCategory useCategory = useItem.GetCategory();

            itemStorage[useCategory] = new Dictionary<int, int>();

            // 使用するレベルの範囲を0で初期化
            for (int level = useItem.GetMinLevel(); level <= useItem.GetMaxLevel(); level++)
            {
                itemStorage[useCategory][level] = 0;
            }
        }
    }

    /// <summary>
    /// アイテムを加算（回収）
    /// </summary>
    public void AddItem(ItemCategory type, int level, int amount = 1)
    {
        if (!itemStorage.ContainsKey(type))
            return;
        //itemStorage[type] = new Dictionary<int, int>();

        if (!itemStorage[type].ContainsKey(level))
            return;
        //itemStorage[type][level] = 0;

        itemStorage[type][level] += amount;
    }

    /// <summary>
    /// 所持数を取得（種類・レベル指定）
    /// </summary>
    public int GetItemCount(ItemCategory type, int level)
    {
        if (itemStorage.ContainsKey(type) && itemStorage[type].ContainsKey(level))
            return itemStorage[type][level];

        return 0;
    }

    /// <summary>
    /// 消費可能か確認
    /// </summary>
    public bool CanConsume(ItemCategory type, int level, int amount = 1)
    {
        return GetItemCount(type, level) >= amount;
    }

    /// <summary>
    /// アイテムを消費（成功時 true、失敗時 false）
    /// </summary>
    public bool ConsumeItem(ItemCategory type, int level, int amount = 1)
    {
        if (!CanConsume(type, level, amount))
            return false;

        itemStorage[type][level] -= amount;
        return true;
    }

    /// <summary>
    /// 必要なアイテムリストすべてがそろっているかチェック
    /// </summary>
    public bool CanConsumeAll(List<ItemRequest> requestList)
    {
        foreach (var request in requestList)
        {
            if (!CanConsume(request.GetCategory(), request.GetLevel(), request.GetValue()))
                return false;
        }
        return true;
    }

    /// <summary>
    /// 条件を満たしていれば一括消費（失敗時は何も消費しない）
    /// </summary>
    public bool ConsumeAll(List<ItemRequest> requestList)
    {
        if (!CanConsumeAll(requestList))
            return false;

        foreach (var request in requestList)
        {
            ConsumeItem(request.GetCategory(), request.GetLevel(), request.GetValue());
        }
        return true;
    }

    /// <summary>
    /// 所持アイテムの内容を表示（デバッグ用）
    /// </summary>
    public void PrintInventory()
    {
        foreach (var typePair in itemStorage)
        {
            foreach (var levelPair in typePair.Value)
            {
                Debug.Log($"[{typePair.Key}] Lv{levelPair.Key} : {levelPair.Value}個");
            }
        }
    }
}
