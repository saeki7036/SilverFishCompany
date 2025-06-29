using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "Scriptable Objects/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    [SerializeField]
    List<UseItem> useItemList;

    public List<UseItem> GetUseItemList() => useItemList;
}

[System.Serializable]
public class UseItem
{
    [SerializeField]
    ItemCategory UseCategory;

    [SerializeField]
    int minLevel = 1;

    [SerializeField]
    int maxLevel = 3;

    public ItemCategory GetCategory() => UseCategory;

    public int GetMinLevel() => minLevel;

    public int GetMaxLevel() => maxLevel;
}
