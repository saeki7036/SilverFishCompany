using UnityEngine;

/// <summary>
/// アイテムのカテゴリを定義する列挙型
/// </summary>
public enum ItemCategory
{
    None,   // なし
    Wood,   // 木材
    Stone,  // 石材
}

/// <summary>
/// アイテムのリクエスト情報を管理するクラス
/// アイテムの種類、レベル、必要数量を保持
/// </summary>
[System.Serializable]
public class ItemRequest
{
    [SerializeField]
    ItemCategory Type;
    [SerializeField]
    int Level = 1;
    [SerializeField]
    int Value = 1;

    /// <summary>
    /// ItemRequestコンストラクタ
    /// </summary>
    /// <param name="type">アイテムカテゴリ</param>
    /// <param name="level">アイテムレベル</param>
    /// <param name="value">必要数量</param>
    public ItemRequest(ItemCategory type, int level, int value)
    {
        Type = type;
        Level = level;
        Value = value;
    }

    /// <summary>
    /// アイテムカテゴリを取得
    /// </summary>
    /// <returns>アイテムカテゴリ</returns>
    public ItemCategory GetCategory() => Type;

    /// <summary>
    /// アイテムレベルを取得
    /// </summary>
    /// <returns>アイテムレベル</returns>
    public int GetLevel() => Level;

    /// <summary>
    /// 必要数量を取得
    /// </summary>
    /// <returns>必要数量</returns>
    public int GetValue() => Value;
}

