using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ベルトアニメーションの設定を管理するScriptableObject
/// アニメーションタイプと名前/インデックスの対応関係を定義し、相互変換機能を提供
/// </summary>
[CreateAssetMenu(fileName = "BeltAnimConfig", menuName = "Scriptable Objects/BeltAnimConfig")]
public class BeltAnimConfig : ScriptableObject
{
    [SerializeField]
    List<AnimNames> animNameInfos = new List<AnimNames>();

    [SerializeField]
    List<AnimIndex> animIndexInfos = new List<AnimIndex>();

    // AnimTypeから名前への変換用辞書
    private Dictionary<AnimType, string> TypeToNameMap;

    // インデックス文字列からAnimTypeへの変換用辞書
    private Dictionary<string, AnimType> indexToTypeMap;

    /// <summary>
    /// 辞書の初期化処理
    /// シリアライズされたリストから高速検索用の辞書を構築
    /// </summary>
    public void Initialize()
    {
        // AnimType → 名前の辞書を初期化
        if (TypeToNameMap == null)
        {
            TypeToNameMap = new();

            TypeToNameMap = animNameInfos.ToDictionary(
            value => value.animNameType,
            value => value.name
            );
        }

        // インデックス → AnimTypeの辞書を初期化
        if (indexToTypeMap == null)
        {
            indexToTypeMap = new();

            indexToTypeMap = animIndexInfos.ToDictionary(
               value => value.index,
               value => value.animIndexType
               );
        }
           
    }

    /// <summary>
    /// AnimTypeからアニメーション名を取得
    /// </summary>
    /// <param name="animType">取得したいアニメーションタイプ</param>
    /// <returns>対応するアニメーション名、存在しない場合は"Multi"</returns>
    public string GetAnimName(AnimType animType)
    {
        return TypeToNameMap.TryGetValue(animType, out string animName) ? animName : "Multi";
    }

    /// <summary>
    /// インデックス文字列からAnimTypeを取得
    /// </summary>
    /// <param name="animIndex">インデックス文字列（例："0000"）</param>
    /// <returns>対応するAnimType、存在しない場合はAnimType.None</returns>
    public AnimType GetAnimType(string animIndex)
    {
        return indexToTypeMap.TryGetValue(animIndex, out var type) ? type : AnimType.None;
    }
}

/// <summary>
/// ベルトアニメーションの種類を定義する列挙型
/// </summary>
public enum AnimType
{
    None,      // 未設定
    straight,  // 直進
    Left,      // 左折
    Right,     // 右折
    Multi      // 複数方向
}

/// <summary>
/// インデックス文字列とAnimTypeの対応関係を定義するクラス
/// </summary>
[System.Serializable]
public class AnimIndex
{
    [Tooltip("アニメーションインデックス（例：0000）")]
    public string index = "0000";

    [Tooltip("対応するアニメーションタイプ")]
    public AnimType animIndexType = AnimType.None;
}

// <summary>
/// AnimTypeとアニメーション名の対応関係を定義するクラス
/// </summary>
[System.Serializable]
public class AnimNames
{
    [Tooltip("アニメーションタイプ")]
    public AnimType animNameType = AnimType.None;

    [Tooltip("実際のアニメーション名")]
    public string name;
}


