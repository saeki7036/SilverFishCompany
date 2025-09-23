using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のスポーン情報を管理するScriptableObject
/// 敵プレハブとウェーブ情報を保持し、スポーンシステムに必要な情報を提供
/// </summary>
[CreateAssetMenu(fileName = "EnemySpownInformation", menuName = "Scriptable Objects/EnemySpownInformation")]
public class EnemySpownInformation : ScriptableObject
{
    [SerializeField]
    [Tooltip("スポーン可能な敵プレハブのリスト")]
    List<GameObject> EnemyPrehabList;

    [SerializeField]
    [Tooltip("ウェーブごとのスポーン設定リスト")]
    List<EnemyWave> WaveList;

    /// <summary>
    /// 全ウェーブの敵スポーン総数を計算
    /// </summary>
    /// <returns>全ウェーブで出現する敵の総数</returns>
    public int EnemySpownALLValue()
    {
        int count = 0;
        foreach (EnemyWave wave in WaveList)
            count += wave.GetSpownCountValue();

        return count;
    }

    /// <summary>
    /// ウェーブの総数を取得
    /// </summary>
    /// <returns>設定されているウェーブ数</returns>
    public int GetWaveCount() => WaveList.Count;

    /// <summary>
    /// 敵IDの最大値を取得（プレハブリストのサイズ）
    /// </summary>
    /// <returns>利用可能な敵IDの最大値</returns>
    public int GetEnemyIDMax() => EnemyPrehabList.Count;

    /// <summary>
    /// 指定インデックスのウェーブ情報を取得
    /// </summary>
    /// <param name="index">ウェーブのインデックス</param>
    /// <returns>ウェーブ情報、範囲外の場合はnull</returns>
    public EnemyWave GetWave(int index)
    {
        if(index < 0 || index >= WaveList.Count)
            return null;

        return WaveList[index];
    }

    /// <summary>
    /// 指定IDの敵プレハブを取得
    /// </summary>
    /// <param name="index">敵ID（プレハブリストのインデックス）</param>
    /// <returns>敵プレハブ、範囲外の場合はnull</returns>
    public GameObject GetPrehab(int index)
    {
        if (index < 0 || index >= EnemyPrehabList.Count)
            return null;

        return EnemyPrehabList[index];
    }
}

/// <summary>
/// 1つのウェーブの敵スポーン設定を定義するクラス
/// スポーンタイミング、数量、敵種類を管理
/// </summary>
[System.Serializable]
public class EnemyWave
{
    [SerializeField]
    [Tooltip("ウェーブの持続時間（秒）")]
    int WaveTimeValue;

    [SerializeField]
    [Tooltip("このウェーブでスポーンする敵の総数")]
    int EnemySpownValue;

    [SerializeField]
    [Tooltip("スポーン可能な敵IDのリスト")]
    List<int> SpownIDList;

    /// <summary>
    /// ウェーブの持続時間を取得
    /// </summary>
    /// <returns>ウェーブ時間（秒）</returns>
    public int GetWaveTime() => WaveTimeValue;

    /// <summary>
    /// このウェーブでスポーンする敵の数を取得
    /// </summary>
    /// <returns>スポーン数</returns>
    public int GetSpownCountValue() => EnemySpownValue;

    /// <summary>
    /// スポーン可能な敵IDリストを取得
    /// </summary>
    /// <returns>敵IDのリスト</returns>
    public List<int> GetSpownID() => SpownIDList;

    /// <summary>
    /// スポーン可能な敵IDからランダムに1つ選択
    /// </summary>
    public int GetRandomEnemyID()
    {
        if (SpownIDList.Count <= 0)
            return -1;

        return SpownIDList[UnityEngine.Random.Range(0, SpownIDList.Count)];
    } 
}
