using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEditor.AssetImporters;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    GamePogressManager gamePogressManager;

    [SerializeField]
    ItemConfig itemConfig;

    [SerializeField]
    List<ItemRequest> StartItemValue;

    ItemTransporter itemTransporter;

    ItemStocker itemStocker;

    float maxTimeCount = 1f;

    static ItemManager instance;

    public static ItemManager Instance => instance;

    public int GetItemValue(ItemCategory category, int level)
    {
        return itemStocker.GetItemCount(category, level);
    }


    public void AddItemStorage(ItemCategory category, int level)
    {
        itemStocker.AddItem(category, level);
    }

    public bool CanConsumeItem(ItemCategory category, int level, int value)
    {
        if(value <= 0)
            return false;

        return itemStocker.CanConsume(category, level, value);
    }

    public bool ItemConsume(ItemCategory category, int level, int value)
    {
        return itemStocker.ConsumeItem(category, level, value);
    }

    public bool CanConsumeAll(List<ItemRequest> requests)
    {
        if (requests == null || requests.Count <= 0)
            return false;

        return itemStocker.CanConsumeAll(requests);
    }

    public bool ItemConsumeAll(List<ItemRequest> requests)
    {
        if (requests == null || requests.Count <= 0)
            return false;

        return itemStocker.ConsumeAll(requests);
    }


    public ProductItem CreateItem(ItemInformation itemInformation,Vector2Int createPos)
    {
        if (itemInformation == null)
            return null;

        ProductItem Item = new ProductItem(itemInformation, createPos, maxTimeCount);

        itemTransporter.AddPool(Item);

        return Item;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        instance = this;

        itemStocker = new ItemStocker(itemConfig);
    }

    private void Start()
    {
        var config = ConfigManager.Instance.GetItemConfig();

        maxTimeCount = config.MaxCount();

        itemTransporter = new ItemTransporter(config.AddCount());

        foreach(var item in StartItemValue)
        {
            itemStocker.AddItem(item.GetCategory(), item.GetLevel(), item.GetValue());
        }
    }

    void FixedUpdate()
    {
        if(gamePogressManager.GetPogressFlag())
            itemTransporter.ItemMovingCheck();
    }
}
