using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// アイテムの設定情報を管理するScriptableObject
/// 使用可能アイテムのカテゴリとレベル範囲を定義
/// </summary>
[CreateAssetMenu(fileName = "ItemConfig", menuName = "Scriptable Objects/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("使用可能なアイテムの設定リスト")]
    List<UseItem> useItemList;

    /// <summary>
    /// 使用可能アイテムのリストを取得
    /// </summary>
    /// <returns>UseItemのリスト</returns>
    public List<UseItem> GetUseItemList() => useItemList;
}

[System.Serializable]
public class UseItem
{
    [SerializeField]
    [Tooltip("アイテムのカテゴリ")]
    ItemCategory UseCategory;

    [SerializeField]
    [Tooltip("このカテゴリの最小レベル")]
    int minLevel = 1;

    [SerializeField]
    [Tooltip("このカテゴリの最大レベル")]
    int maxLevel = 3;

    /// <summary>
    /// アイテムカテゴリを取得
    /// </summary>
    /// <returns>アイテムカテゴリ</returns>
    public ItemCategory GetCategory() => UseCategory;

    /// <summary>
    /// 最小レベルを取得
    /// </summary>
    /// <returns>最小レベル値</returns>
    public int GetMinLevel() => minLevel;

    /// <summary>
    /// 最大レベルを取得
    /// </summary>
    /// <returns>最大レベル値</returns>
    public int GetMaxLevel() => maxLevel;
}
