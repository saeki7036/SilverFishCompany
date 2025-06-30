using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpownInformation", menuName = "Scriptable Objects/EnemySpownInformation")]
public class EnemySpownInformation : ScriptableObject
{
    [SerializeField]
    List<GameObject> EnemyPrehabList;

    [SerializeField]
    List<EnemyWave> WaveList;

    public int EnemySpownALLValue()
    {
        int count = 0;
        foreach (EnemyWave wave in WaveList)
            count += wave.GetSpownCountValue();

        return count;
    }

    public int GetWaveCount() => WaveList.Count;

    public  int GetEnemyIDMax() => EnemyPrehabList.Count;

    public EnemyWave GetWave(int index)
    {
        if(index < 0 || index >= WaveList.Count)
            return null;

        return WaveList[index];
    }

    public GameObject GetPrehab(int index)
    {
        if (index < 0 || index >= EnemyPrehabList.Count)
            return null;

        return EnemyPrehabList[index];
    }
}

[System.Serializable]
public class EnemyWave
{
    [SerializeField]
    int WaveTimeValue;

    [SerializeField]
    int EnemySpownValue;

    [SerializeField]
    List<int> SpownIDList;

    public int GetWaveTime() => WaveTimeValue;

    public int GetSpownCountValue() => EnemySpownValue;

    public List<int> GetSpownID() => SpownIDList;

    public int GetRandomEnemyID()
    {
        if (SpownIDList.Count <= 0)
            return -1;

        return SpownIDList[UnityEngine.Random.Range(0, SpownIDList.Count)];
    } 
}
