using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInformation", menuName = "Scriptable Objects/ItemInformation")]
public class ItemInformation : ScriptableObject
{
    public ItemCategory itemCategory;
    public int itemLevel;
    public GameObject itemPrehab;

    // プロパティ
    public ItemCategory ItemCategory
    {
        get => itemCategory;
    }

    public int ItemLevel
    {
        get => itemLevel;
    }

    public GameObject ItemPrehab
    {
        get => itemPrehab;
    }
}

public enum ItemCategory
{
    None,
    Wood,
    storn,
}
