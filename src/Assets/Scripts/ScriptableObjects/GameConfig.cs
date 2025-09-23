using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ゲームの基本設定を管理するScriptableObject
/// アイテム輸送とビルディングの設定を統合管理
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("アイテム輸送に関する設定")]
    ItemTransportConfig itemTransportConfig;

    [SerializeField]
    [Tooltip("ビルディングに関する設定")]
    BuildingConfig buildingConfig;

    // <summary>
    /// アイテム輸送設定を取得
    /// </summary>
    /// <returns>アイテム輸送設定</returns>
    public ItemTransportConfig GetItemConfig ()=> itemTransportConfig;
  
    /// <summary>
    /// ビルディング設定を取得
    /// </summary>
    /// <returns>ビルディング設定</returns>
    public BuildingConfig GetBuildingConfig ()=> buildingConfig;
}

/// <summary>
/// アイテム輸送システムの設定を管理するクラス
/// 輸送量の増加値と最大値を定義
/// </summary>
[System.Serializable]
public class ItemTransportConfig
{
    [SerializeField]
    [Tooltip("輸送量の増加値（毎フレーム/毎秒）")]
    float addCountValue = 0.02f;

    [SerializeField]
    [Tooltip("輸送量の最大値")]
    float maxCountValue = 1f;

    /// <summary>
    /// 輸送量の増加値を取得
    /// </summary>
    /// <returns>増加値</returns>
    public float AddCount() => addCountValue;

    /// <summary>
    /// 輸送量の最大値を取得
    /// </summary>
    /// <returns>最大値</returns>
    public float MaxCount() => maxCountValue;
}

/// <summary>
/// ビルディングシステムの設定を管理するクラス
/// 操作カウントの設定と各建物タイプの起動時間を管理
/// </summary>
[System.Serializable]
public class BuildingConfig
{
    [SerializeField]
    [Tooltip("操作カウントの増加値")]
    float addOperatCountValue = 0.02f;

    [SerializeField]
    [Tooltip("利用可能な建物タイプとその設定")]
    List<BuildingParamater> useCellTypes;

    // 起動時間を取得するヘルパー
    private Dictionary<BuildType, float> operatHelper;

    /// <summary>
    /// 操作カウント値を取得
    /// </summary>
    /// <returns>操作カウント増加値</returns>
    public float GetOperatCount() => addOperatCountValue;

    /// <summary>
    /// 利用可能な建物タイプ一覧を取得
    /// </summary>
    /// <returns>建物タイプのリスト</returns>
    public List<BuildType> GetCellTypes() => useCellTypes.Select(p => p.type).ToList();

    /// <summary>
    /// 指定された建物タイプの起動時間を取得
    /// </summary>
    /// <param name="type">建物タイプ</param>
    /// <returns>起動時間、存在しない場合は0f</returns>
    public float GetStartupTime(BuildType type)
    {
        // 初回アクセス時に辞書を構築（遅延初期化）
        if (operatHelper == null || operatHelper.Count == 0)
        {
            operatHelper = new Dictionary<BuildType, float>();

            foreach (BuildingParamater config in useCellTypes)
                operatHelper[config.type] = config.startupTime;
        }

        return operatHelper.TryGetValue(type, out var time) ? time : 0f;
    }
}

// <summary>
/// 建物タイプとその設定パラメータを定義する構造体
/// </summary>
[System.Serializable]
public struct BuildingParamater
{
    [Tooltip("建物の種類")]
    public BuildType type;

    [Tooltip("建物の起動時間または生産周期")]
    public float startupTime;  // 起動にかかる時間や生産周期など
}

