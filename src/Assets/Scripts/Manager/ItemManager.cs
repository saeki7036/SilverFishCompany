using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // アイテムシステム全体を統括管理するマネージャークラス（シングルトン）

    [SerializeField]
    GamePogressManager gamePogressManager; // ゲーム進行状態を管理するマネージャー

    [SerializeField]
    ItemConfig itemConfig; // アイテムの設定情報

    [SerializeField]
    List<ItemRequest> StartItemValue; // ゲーム開始時の初期アイテム設定

    float maxTimeCount = 1f;// アイテム輸送の最大時間
    ItemTransporter itemTransporter;// アイテム輸送システム
    ItemStocker itemStocker;// アイテムストック管理システム

    static ItemManager instance;// シングルトンインスタンス

    /// <summary>
    /// シングルトンインスタンスへのアクセサ
    /// </summary>
    [HideInInspector]
    public static ItemManager Instance => instance;

    /// <summary>
    /// 指定されたカテゴリとレベルのアイテム数を取得
    /// </summary>
    /// <param name="category">アイテムカテゴリ</param>
    /// <param name="level">アイテムレベル</param>
    /// <returns>所持しているアイテム数</returns>
    public int GetItemValue(ItemCategory category, int level)
    {
        return itemStocker.GetItemCount(category, level);
    }

    /// <summary>
    /// 指定されたアイテムをストックに追加
    /// </summary>
    /// <param name="category">アイテムカテゴリ</param>
    /// <param name="level">アイテムレベル</param>
    public void AddItemStorage(ItemCategory category, int level)
    {
        itemStocker.AddItem(category, level);
    }

    /// <summary>
    /// 指定されたアイテムを消費可能かどうかを判定
    /// </summary>
    /// <param name="category">アイテムカテゴリ</param>
    /// <param name="level">アイテムレベル</param>
    /// <param name="value">消費したい数量</param>
    /// <returns>消費可能な場合true</returns>
    public bool CanConsumeItem(ItemCategory category, int level, int value)
    {
        if(value <= 0)
            return false;

        return itemStocker.CanConsume(category, level, value);
    }

    /// <summary>
    /// 指定されたアイテムを実際に消費
    /// </summary>
    /// <param name="category">アイテムカテゴリ</param>
    /// <param name="level">アイテムレベル</param>
    /// <param name="value">消費する数量</param>
    /// <returns>消費成功時true</returns>
    public bool ItemConsume(ItemCategory category, int level, int value)
    {
        return itemStocker.ConsumeItem(category, level, value);
    }

    /// <summary>
    /// 複数のアイテムを一括で消費可能かどうかを判定
    /// </summary>
    /// <param name="requests">消費したいアイテムのリスト</param>
    /// <returns>全て消費可能な場合true</returns>
    public bool CanConsumeAll(List<ItemRequest> requests)
    {
        if (requests == null || requests.Count <= 0)
            return false;

        return itemStocker.CanConsumeAll(requests);
    }

    /// <summary>
    /// 複数のアイテムを一括で消費
    /// </summary>
    /// <param name="requests">消費するアイテムのリスト</param>
    /// <returns>全て消費成功時true</returns>
    public bool ItemConsumeAll(List<ItemRequest> requests)
    {
        if (requests == null || requests.Count <= 0)
            return false;

        return itemStocker.ConsumeAll(requests);
    }

    /// <summary>
    /// 新しいアイテムを生成し、輸送システムに登録
    /// </summary>
    /// <param name="itemInformation">生成するアイテムの情報</param>
    /// <param name="createPos">生成位置</param>
    /// <returns>生成されたProductItemオブジェクト</returns>
    public ProductItem CreateItem(ItemInformation itemInformation,Vector2Int createPos)
    {
        // 情報のヌルチェック
        if (itemInformation == null)
            return null;

        // アイテムクラス作成
        ProductItem Item = new ProductItem(itemInformation, createPos, maxTimeCount);

        //アイテム輸送Listに登録
        itemTransporter.AddPool(Item);

        return Item;
    }

    void Awake()
    {
        // シングルトンパターンの実装
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        // シングルトン初期化
        instance = this;

        // アイテムストック管理システムを初期化
        itemStocker = new ItemStocker(itemConfig);
    }

    void Start()
    {
        // 設定の読み込みと初期アイテムの設定

        // ItemTransportConfig 取得
        ItemTransportConfig config = ConfigManager.Instance.GetItemConfig();

        // 輸送時間の最大値を設定から取得
        maxTimeCount = config.MaxCount();

        // アイテム輸送システムを初期化
        itemTransporter = new ItemTransporter(config.AddCount());

        // 初期アイテムをストックに追加
        foreach (var item in StartItemValue)
        {
            itemStocker.AddItem(item.GetCategory(), item.GetLevel(), item.GetValue());
        }
    }

    void FixedUpdate()
    {
        // アイテム輸送の更新処理
        if (gamePogressManager.GetPogressFlag())// ゲーム進行中のみ
            itemTransporter.ItemMovingCheck();// 移動チェックを実行
    }

#if UNITY_EDITOR
    /// <summary>
    /// デバッグ用のアイテム追加機能（エディタ専用）
    /// U+Yキーでアイテムを追加
    /// </summary>
    void Update()
    {
        if(Input.GetKey(KeyCode.U) && Input.GetKeyDown(KeyCode.Y))
        {
            itemStocker.AddItem(ItemCategory.Wood, 1, 50);
            itemStocker.AddItem(ItemCategory.Stone, 1, 50);
        }
    }
#endif
}
