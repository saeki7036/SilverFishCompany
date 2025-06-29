using UnityEngine;

public enum ItemCategory
{
    None,
    Wood,
    Stone,
}

[System.Serializable]
public class ItemRequest
{
    [SerializeField]
    ItemCategory Type;
    [SerializeField]
    int Level = 1;
    [SerializeField]
    int Value = 1;

    public ItemRequest(ItemCategory type, int level, int value)
    {
        Type = type;
        Level = level;
        Value = value;
    }

    public ItemCategory GetCategory() => Type;

    public int GetLevel() => Level;

    public int GetValue() => Value;
}

