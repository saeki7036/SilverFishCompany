using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    // ゲーム設定データの管理を行うマネージャークラス(シングルトン)

    /// <summary>
    /// ゲームの設定データを格納
    /// </summary>
    [SerializeField]
    GameConfig gameConfig;

    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    static ConfigManager instance;

    /// <summary>
    /// ConfigManagerのインスタンスのアクセサ
    /// </summary>
    public static ConfigManager Instance => instance;

    /// <summary>
    /// アイテム輸送設定データを取得
    /// </summary>
    /// <returns>アイテム輸送に関する設定データ</returns>
    public ItemTransportConfig GetItemConfig() => gameConfig.GetItemConfig();

    /// <summary>
    /// 建物設定データを取得
    /// </summary>
    /// <returns>建物に関する設定データ</returns>
    public BuildingConfig GetBuildingConfig() => gameConfig.GetBuildingConfig();

    void Awake()
    {
        // 既にインスタンスが存在し、それが自分でない場合は破棄
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        // 自分をシングルトンインスタンスとして設定
        instance = this;
    }
}
