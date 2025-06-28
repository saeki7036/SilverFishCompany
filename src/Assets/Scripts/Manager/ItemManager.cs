using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    GamePogressManager gamePogressManager;

    static ItemManager instance;

    public static ItemManager Instance => instance;

    public void AddListItem(ProductItem productItem) => ItemSlot.Add(productItem);

    public float GetMaxTimeCount() => maxTimeCount;

    List<ProductItem> ItemSlot;
    float addTimeCount = 0.02f;
    float maxTimeCount = 1.5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        instance = this;

        ItemSlot = new List<ProductItem>();
    }

    private void Start()
    {
        var config = ConfigManager.Instance.GetItemConfig();
        addTimeCount = config.AddCount();
        maxTimeCount = config.MaxCount();
    }

    void FixedUpdate()
    {
        if(gamePogressManager.GetPogressFlag())
            ItemMovingCheck();
    }

    void ItemMovingCheck()
    {
        // Removeを扱うので逆順ループ
        for (int i = ItemSlot.Count - 1; i >= 0; i--)
        {
            // nullチェック
            if (ItemSlot[i] == null)   
            {
                ItemSlot.RemoveAt(i);
                continue;
            }

            // オブジェクトがなければ削除する
            if(ItemSlot[i].IsEnptyItemObject())
            {
                ItemSlot.RemoveAt(i);
                continue;
            }
            
            // アイテム運搬処理
            if (ItemSlot[i].IsItemMove())
            {
                ItemSlot[i].ItemMovement(addTimeCount);
            }
        }
    }
}
