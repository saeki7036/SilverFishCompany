using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyManagerTest : MonoBehaviour
{
    [SerializeField]
    GamePogressManager gamePogressManager;

    [SerializeField]
    int[] Spowntime;

    [SerializeField]
    GameObject[] EnemyPrehab;

    [SerializeField]
    Vector2 minPos;

    [SerializeField]
    Vector2 maxPos;

    [SerializeField]
    public UnityEvent ClearIvent;

    [SerializeField]
    Text EnemyCounter;

    [SerializeField]
    EnemySpownInformation enemyInformation;

    [SerializeField]
    Text DebugText;

    [HideInInspector]
    static EnemyManagerTest instance;

    [HideInInspector]
    public static EnemyManagerTest Instance => instance;

    List<EnemyTest> enemyList;

    int RemoveCount = 0;

    int timeCount = 0;
    
    int WaveIndex = 0;

    int nextSpownCount = 0;

    int nextIndexCount = 0;

    int currentEnemySpownValue = 0;

    EnemyWave currentWave;

    bool IventOneFlag;

    public Vector2 NearestPos(Vector2 basePos ,float range)
    {
        // float rangePow = range * range;

        Vector2 nearest = -Vector2.one;
        float nearestDistance = range * range;

        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null)
            {
                continue;
            }
            else
            {
                float distancePow = DistancePow(enemyList[i].GetCurrentPos(), basePos);

                if (distancePow < nearestDistance)
                {
                    nearestDistance = distancePow;
                    nearest = enemyList[i].GetCurrentPos();
                }
            }
        }
        
        return nearest;
    }

    float DistancePow(Vector2 a, Vector2 b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        return num * num + num2 * num2;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IventOneFlag = true;
        WaveIndex = 0;
        RemoveCount = 0;
        timeCount = 0;
        nextIndexCount = 0;
        nextSpownCount = 0;
        enemyList = new List<EnemyTest>();
        SetWave();
        UpdateText();
    }

    void DebugTextRead()
    {
        DebugText.text = "timeCount:" + timeCount + "\n" +
                         "nextSpownCount:" + nextSpownCount + "\n" +
                         "nextIndexCount:" + nextIndexCount + "\n" +
                         "currentEnemySpown:" + currentEnemySpownValue + "\n" +                   
                         "WaveIndex:" + WaveIndex + "\n";                        
    }


    void SetWave()
    {
        if (WaveIndex >= enemyInformation.GetWaveCount())
            return;

        currentWave = enemyInformation.GetWave(WaveIndex);
        nextIndexCount += currentWave.GetWaveTime();
        currentEnemySpownValue = currentWave.GetSpownCountValue();
    }

    Vector2 SpownPos() => new Vector2()
    {
        x = UnityEngine.Random.Range(minPos.x, maxPos.x),
        y = UnityEngine.Random.Range(minPos.y, maxPos.y),
    };


    private void Update()
    {
        //enemyList.RemoveAll(enemy => enemy == null);
        DebugTextRead();
        //Debug.Log(enemyList.Count + " : " + index + " : " + Spowntime.Length);

        if (IventOneFlag && RemoveCount == enemyInformation.EnemySpownALLValue()) 
        {
            IventOneFlag = false;
            ClearIvent?.Invoke();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gamePogressManager.GetPogressFlag())
            return;

        timeCount++;

        if(currentEnemySpownValue > 0 && 
           timeCount >= nextSpownCount)
        {
            nextSpownCount = UnityEngine.Random.Range(nextSpownCount, nextIndexCount);

            currentEnemySpownValue--;

            SpownEnemy();
        }

        if(currentEnemySpownValue <= 0 && 
           WaveIndex < enemyInformation.GetWaveCount() && 
           timeCount >= nextIndexCount)
        {
            WaveIndex++;

            int beforeIndexCount = nextIndexCount;
            SetWave();
            nextSpownCount = UnityEngine.Random.Range(beforeIndexCount,(beforeIndexCount + nextIndexCount / 2));
        }

        EnemyListUpdate();
    }

    
    void SpownEnemy()
    {
        int enemyID = currentWave.GetRandomEnemyID();

        if (enemyID == -1) 
            return;

        GameObject enemyPrehab = enemyInformation.GetPrehab(enemyID);

        if (enemyPrehab == null)
            return;

        GameObject enemyObject = Instantiate(enemyPrehab, SpownPos(),Quaternion.Euler(0f, 0f, 180f));

        enemyObject.transform.parent = transform;

        enemyList.Add(enemyObject.GetComponent<EnemyTest>());
    }

    void EnemyListUpdate()
    {
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null)
            {
                RemoveCount++;
                enemyList.RemoveAt(i);
                UpdateText();
            }
            else
            {
                enemyList[i].FixedUpdates();
            }
        }
    }

    void UpdateText()
    { 
        EnemyCounter.text = RemoveCount.ToString() + "/" + enemyInformation.EnemySpownALLValue().ToString();
    }
}
