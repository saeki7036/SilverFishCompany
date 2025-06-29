using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    List<EnemyTest> enemyList;

    public int timeCount;

    public int index = 0;

    [HideInInspector]
    static EnemyManagerTest instance;

    [HideInInspector]
    public static EnemyManagerTest Instance => instance;

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
        timeCount = 0;
        enemyList = new List<EnemyTest>();
    }

    private void Update()
    {
        enemyList.RemoveAll(enemy => enemy == null);

        Debug.Log(enemyList.Count + " : " + index + " : " + Spowntime.Length);

        if(enemyList.Count == 0 && index == Spowntime.Length) 
        {
            ClearIvent?.Invoke();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gamePogressManager.GetPogressFlag())
            return;

        timeCount++;

        if (index != Spowntime.Length && Spowntime[index] < timeCount)
        {
            index++;
            Vector2 pos = new()
            {
                x = UnityEngine.Random.Range(minPos.x, maxPos.x),
                y = UnityEngine.Random.Range(minPos.y, maxPos.y)
            };

            GameObject gameObject = Instantiate(
                EnemyPrehab[UnityEngine.Random.Range(0, EnemyPrehab.Length)],
                pos,
                Quaternion.Euler(0f,0f,180f));

            gameObject.transform.parent = transform;

            enemyList.Add(gameObject.GetComponent<EnemyTest>());
        }

        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i);
            }
            else
            {
                enemyList[i].FixedUpdates();
            }
        }
    }
}
