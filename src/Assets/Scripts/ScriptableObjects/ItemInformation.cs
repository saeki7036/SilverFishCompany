using UnityEngine;

/// <summary>
/// 個別アイテムの詳細情報を管理するScriptableObject
/// アイテムのカテゴリ、レベル、プレハブを定義し、アイテムシステムの基本単位として機能
/// </summary>
[CreateAssetMenu(fileName = "ItemInformation", menuName = "Scriptable Objects/ItemInformation")]
public class ItemInformation : ScriptableObject
{
    [SerializeField]
    [Tooltip("アイテムが属するカテゴリ")]
    ItemCategory itemCategory;

    [SerializeField]
    [Tooltip("アイテムのレベル（強さや等級を表す）")]
    int itemLevel;

    [SerializeField]
    [Tooltip("ゲーム内で生成されるアイテムのプレハブ")]
    GameObject itemPrehab;

    // <summary>
    /// アイテムカテゴリを取得
    /// </summary>
    /// <returns>このアイテムのカテゴリ</returns>
    public ItemCategory GetItemCategory() => itemCategory;

    /// <summary>
    /// アイテムレベルを取得
    /// </summary>
    /// <returns>このアイテムのレベル</returns>
    public int GetItemLevel() => itemLevel;

    /// <summary>
    /// アイテムプレハブを取得
    /// </summary>
    /// <returns>このアイテムのプレハブGameObject</returns>
    public GameObject GetItemPrehab() => itemPrehab;
}
