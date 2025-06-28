using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    [SerializeField]
    GameConfig gameConfig;

    static ConfigManager instance;
    public static ConfigManager Instance => instance;

    public ItemConfig GetItemConfig() => gameConfig.GetItemConfig();

    public BuildingConfig GetBuildingConfig() => gameConfig.GetBuildingConfig();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        instance = this;
    }
}
