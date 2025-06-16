using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTimeConfig", menuName = "Scriptable Objects/BuildingTimeConfig")]
public class BuildingTimeConfig : ScriptableObject
{
    public List<FacilityConfig> startupConfigs;

    // 起動時間を取得するヘルパー
    private Dictionary<CellType, float> operatHelper;

    public float GetStartupTime(CellType type)
    {
        if (operatHelper == null || operatHelper.Count == 0)
        {
            operatHelper = new Dictionary<CellType, float>();
            foreach (var config in startupConfigs)
                operatHelper[config.type] = config.startupTime;
        }

        return operatHelper.TryGetValue(type, out var time) ? time : 0f;
    }
}

[System.Serializable]
public struct FacilityConfig
{
    public CellType type;
    public float startupTime;  // 起動にかかる時間や生産周期など
}

