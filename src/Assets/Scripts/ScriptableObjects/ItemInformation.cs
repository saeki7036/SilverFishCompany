using UnityEngine;

[CreateAssetMenu(fileName = "ItemInformation", menuName = "Scriptable Objects/ItemInformation")]
public class ItemInformation : ScriptableObject
{
    public ItemCategory itemCategory;
    public int itemLevel;
    public GameObject itemPrehab;

    // プロパティ
    public ItemCategory GetItemCategory() => itemCategory;
    

    public int GetItemLevel() => itemLevel;
    

    public GameObject GetItemPrehab() => itemPrehab;
}

public enum ItemCategory
{
    None,
    Wood,
    Stone,
}
