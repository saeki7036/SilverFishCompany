using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyManagerTest : MonoBehaviour
{
    // 敵管理するクラスのテスト(シングルトン)
    // wave制のスポーンシステムは本採用予定

    [SerializeField]
    GamePogressManager gamePogressManager;

    [SerializeField]
    int[] Spowntime;　// 未使用

    [SerializeField]
    GameObject[] EnemyPrehab; // 未使用

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

    [SerializeField]
    GameObject BaseCamp;

    [HideInInspector]
    static EnemyManagerTest instance;

    [HideInInspector]
    public static EnemyManagerTest Instance => instance;

    Vector2 BaseCampPosSenter;
    HashSet<Vector2Int> BaseCampPos;

    public Vector2 GetBaseCampPosSenter() => BaseCampPosSenter;

    public HashSet<Vector2Int> GetBaseCampPos() => BaseCampPos;

    List<EnemyBase> enemyList;// 生成された敵のリスト
  
    int RemoveCount = 0;// 撃破された敵の数  
    int timeCount = 0;// 経過時間カウンタ   
    int WaveIndex = 0;// 現在のWaveインデックス   
    int nextSpownCount = 0;// 次の敵スポーンタイミング
    int nextIndexCount = 0;// 次のWave開始タイミング
    int currentEnemySpownValue = 0;// 現在のWaveで残りスポーン数

    EnemyWave currentWave;// 現在のWave情報

    bool IventOneFlag;// クリアイベント実行フラグ（一度だけ実行）

    /// <summary>
    /// 指定位置から範囲内にいる最も近い敵の位置を取得
    /// タレットの攻撃対象選定などで使用
    /// </summary>
    /// <param name="basePos">基準位置</param>
    /// <param name="range">検索範囲</param>
    /// <returns>最も近い敵の位置、見つからない場合は(-1,-1)</returns>
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
                // 距離の二乗を計算
                float distancePow = DistancePow(enemyList[i].transform.position, basePos);

                if (distancePow < nearestDistance)
                {
                    nearestDistance = distancePow;
                    nearest = enemyList[i].transform.position;
                }
            }
        }
        
        return nearest;
    }

    /// <summary>
    /// 2点間の距離の二乗を計算
    /// 平方根計算を省略して処理を高速化
    /// </summary>
    /// <param name="a">座標A</param>
    /// <param name="b">座標B</param>
    /// <returns>距離の二乗</returns>
    float DistancePow(Vector2 a, Vector2 b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        return num * num + num2 * num2;
    }

    void Awake()
    {
        // シングルトンパターンの初期化
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
        // 初期化処理
        IventOneFlag = true;
        WaveIndex = 0;
        RemoveCount = 0;
        timeCount = 0;
        nextIndexCount = 0;
        nextSpownCount = 0;
        enemyList = new List<EnemyBase>();
        BaseCampPos = new HashSet<Vector2Int>();

        Vector2Int BaseCampMinPos = new((int)BaseCamp.transform.position.x, (int)BaseCamp.transform.position.y);

        BaseCampBuilding baseCampBuilding = (BaseCampBuilding)GridMapManager.Instance.GetCell(BaseCampMinPos).GetBuilding();

        BaseCampPosSenter = baseCampBuilding.GetBuidingPosSenter();
        BaseCampPos = baseCampBuilding.GetVectorIntGridPos();

        // 最初のWave設定
        SetWave();
        UpdateText();
    }

    /// <summary>
    /// デバッグ情報をテキストに表示
    /// 開発時の状態確認用
    /// </summary>
    void DebugTextRead()
    {
        DebugText.text = "timeCount:" + timeCount + "\n" +
                         "nextSpownCount:" + nextSpownCount + "\n" +
                         "nextIndexCount:" + nextIndexCount + "\n" +
                         "currentEnemySpown:" + currentEnemySpownValue + "\n" +                   
                         "WaveIndex:" + WaveIndex + "\n";                        
    }

    /// <summary>
    /// Wave情報を設定
    /// 次のWaveの開始時間と敵スポーン数を設定
    /// </summary>
    void SetWave()
    {
        // 全Waveが終了している場合は処理しない
        if (WaveIndex >= enemyInformation.GetWaveCount())
            return;

        currentWave = enemyInformation.GetWave(WaveIndex);// 現在のWave情報を取得
        nextIndexCount += currentWave.GetWaveTime();// 次のWave開始タイミングを設定
        currentEnemySpownValue = currentWave.GetSpownCountValue();// 現在のWaveでスポーンする敵の数を設定
    }

    /// <summary>
    /// ランダムなスポーン位置を生成
    /// 設定された範囲内でランダムに座標を決定
    /// </summary>
    /// <returns>スポーン座標</returns>
    Vector2 SpownPos() => new Vector2()
    {
        x = UnityEngine.Random.Range(minPos.x, maxPos.x),
        y = UnityEngine.Random.Range(minPos.y, maxPos.y),
    };


    private void Update()
    {
        //enemyList.RemoveAll(enemy => enemy == null);
        //Debug.Log(enemyList.Count + " : " + index + " : " + Spowntime.Length);
        //Debug表示
        DebugTextRead();

        // 全敵撃破でクリアイベント実行（一度だけ）
        if (IventOneFlag && RemoveCount == enemyInformation.EnemySpownALLValue()) 
        {
            IventOneFlag = false;
            ClearIvent?.Invoke();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ゲーム進行が停止中は処理しない
        if (!gamePogressManager.GetPogressFlag())
            return;

        timeCount++;

        // 敵スポーン処理
        if (currentEnemySpownValue > 0 && 
           timeCount >= nextSpownCount)
        {
            // 次のスポーンタイミングをランダムに設定
            nextSpownCount = UnityEngine.Random.Range(nextSpownCount, nextIndexCount);

            currentEnemySpownValue--;

            SpownEnemy();
        }

        // Wave進行処理
        if (currentEnemySpownValue <= 0 && 
           WaveIndex < enemyInformation.GetWaveCount() && 
           timeCount >= nextIndexCount)
        {
            WaveIndex++;

            // 次のWaveの開始準備
            int beforeIndexCount = nextIndexCount;

            SetWave();

            // Wave間のインターバル考慮して次のスポーンタイミングを設定
            nextSpownCount = UnityEngine.Random.Range(beforeIndexCount,(beforeIndexCount + nextIndexCount / 2));
        }

        EnemyListUpdate();
    }

    /// <summary>
    /// 敵をスポーンする処理
    /// 現在のWaveから敵タイプをランダム選択して生成
    /// </summary>
    void SpownEnemy()
    {
        // 現在のWaveからランダムに敵IDを取得
        int enemyID = currentWave.GetRandomEnemyID();

        if (enemyID == -1)
            return;

        // 敵のプレハブを取得
        GameObject enemyPrehab = enemyInformation.GetPrehab(enemyID);

        if (enemyPrehab == null)
            return;

        // 敵オブジェクトを生成（180度回転で配置）
        GameObject enemyObject = Instantiate(enemyPrehab, SpownPos(),Quaternion.Euler(0f, 0f, 180f));

        // 親オブジェクトに設定（階層整理）
        enemyObject.transform.parent = transform;

        // 敵クラス取得
        EnemyBase enemyBase = enemyObject.GetComponent<EnemyBase>();

        //敵のStartで行動AIやHPバーなどの、呼ぶものを呼び出し
        enemyBase.EnemyStart();

        // 敵リストに追加
        enemyList.Add(enemyBase);
    }

    /// <summary>
    /// 敵リストの更新処理
    /// 破壊された敵の削除と生存敵の更新処理
    /// </summary>
    void EnemyListUpdate()
    {
        // 逆順ループで削除処理を安全に実行
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null)
            {
                // 撃破カウントを増加
                RemoveCount++;

                // リストから削除
                enemyList.RemoveAt(i);

                // UI更新
                UpdateText();
            }
            else
            {
                // 生存している敵の更新処理
                enemyList[i].EnemyUpdate();
            }
        }
    }

    /// <summary>
    /// 敵撃破数の表示を更新
    /// 進行状況をUIに反映
    /// </summary>
    void UpdateText()
    { 
        EnemyCounter.text = RemoveCount.ToString() + "/" + enemyInformation.EnemySpownALLValue().ToString();
    }
}
