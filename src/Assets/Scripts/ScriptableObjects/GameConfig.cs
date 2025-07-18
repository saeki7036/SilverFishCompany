﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField]
    ItemTransportConfig itemTransportConfig;

    public ItemTransportConfig GetItemConfig ()=> itemTransportConfig;

    [SerializeField]
    BuildingConfig buildingConfig;

    public BuildingConfig GetBuildingConfig ()=> buildingConfig;
}

[System.Serializable]
public class ItemTransportConfig
{
    [SerializeField]
    float addCountValue = 0.02f;
    [SerializeField]
    float maxCountValue = 1f;

    public float AddCount() => addCountValue;
    public float MaxCount() => maxCountValue;
}

[System.Serializable]
public class BuildingConfig
{
    [SerializeField]
    float addOperatCountValue = 0.02f;

    public float GetOperatCount() => addOperatCountValue;

    [SerializeField]
    List<BuildingParamater> useCellTypes;

    public List<BuildType> GetCellTypes() => useCellTypes.Select(p => p.type).ToList();

    // 起動時間を取得するヘルパー
    private Dictionary<BuildType, float> operatHelper;

    public float GetStartupTime(BuildType type)
    {
        if (operatHelper == null || operatHelper.Count == 0)
        {
            operatHelper = new Dictionary<BuildType, float>();
            foreach (var config in useCellTypes)
                operatHelper[config.type] = config.startupTime;
        }

        return operatHelper.TryGetValue(type, out var time) ? time : 0f;
    }
}

[System.Serializable]
public struct BuildingParamater
{
    public BuildType type;
    public float startupTime;  // 起動にかかる時間や生産周期など
}

