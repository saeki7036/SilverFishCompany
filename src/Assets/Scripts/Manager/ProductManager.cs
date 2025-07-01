using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    // 各建物タイプの生産タイマーを管理し、時間経過に応じて建物を動作させるマネージャークラス

    [SerializeField]
    GamePogressManager gamePogressManager; //ゲーム進行を管理するマネージャー

    // タイマーカウントの増加値（FixedUpdateごとに加算される値）
    float addTimeCountValue = 0.02f;

    // 各建物タイプの生産タイマーを管理する辞書
    Dictionary<BuildType, ProductTimer> ProductOperater;

    /// <summary>
    /// 建物設定を取得
    /// </summary>
    /// <returns>BuildingConfig設定</returns>
    BuildingConfig GetConfig() => ConfigManager.Instance.GetBuildingConfig();

    /// <summary>
    /// 指定された建物タイプの動作時間を取得
    /// </summary>
    /// <param name="cellType">建物タイプ</param>
    /// <returns>動作に必要な時間</returns>
    float GetOperatTime(BuildType cellType) => GetConfig().GetStartupTime(cellType);

    /// <summary>
    /// 個別建物タイプの生産タイマーを管理するクラス
    /// 時間カウント、動作判定、リセット機能を提供
    /// </summary>
    class ProductTimer
    {
        BuildType cellType;          // 建物タイプ
        float timeCount;            // 現在の時間カウント
        float operatCount;          // 動作に必要な時間

        /// <summary>
        /// ProductTimerのコンストラクタ
        /// </summary>
        /// <param name="cellType">建物タイプ</param>
        /// <param name="operatCount">動作に必要な時間</param>
        public ProductTimer(BuildType cellType, float operatCount)
        {
            this.cellType = cellType;
            this.operatCount = operatCount;

            timeCount = 0f;
        }

        /// <summary>
        /// 建物タイプを取得
        /// </summary>
        /// <returns>この タイマーが管理する建物タイプ</returns>
        public BuildType GetCellType() => cellType;

        /// <summary>
        /// 時間カウントを増加
        /// </summary>
        /// <param name="count">増加する時間</param>
        public void AddCount(float count) => timeCount += count;

        /// <summary>
        /// 時間カウントをリセット
        /// </summary>
        public void CountReset() => timeCount = 0f;

        /// <summary>
        /// 動作可能かどうかを判定
        /// </summary>
        /// <returns>動作可能な場合true</returns>
        public bool IsOperat() => timeCount >= operatCount;
    }

    void Start()
    {
        // 初期化処理

        var config = GetConfig();

        // 設定からタイマー増加値を取得
        addTimeCountValue = config.GetOperatCount();
           
        ProductOperater = new Dictionary<BuildType, ProductTimer>();

        // 各建物タイプに対応するタイマーを生成
        foreach (BuildType type in config.GetCellTypes())
        {
            ProductOperater[type] = new ProductTimer(type,GetOperatTime(type));
        }
    }

    void FixedUpdate()
    {
        // ゲームの進行中のみタイマーを更新
        if (gamePogressManager.GetPogressFlag())
        {
            AddTimeCount();
            OperatCheck();
        }
    }

    /// <summary>
    /// 全ての生産タイマーの時間カウントを増加
    /// </summary>
    void AddTimeCount()
    {
        foreach (ProductTimer product in ProductOperater.Values)
        {
            product.AddCount(addTimeCountValue);
        }
    }

    /// <summary>
    /// 各タイマーをチェックし、可能な場合は建物を動作させる
    /// </summary>
    void OperatCheck()
    {
        foreach (ProductTimer product in ProductOperater.Values)
        {
            if (product.IsOperat())
            {
                // 動作後はタイマーをリセット
                product.CountReset();
                // 建物を動作
                GridMapManager.Instance.OperatBuilding(product.GetCellType());
            }
        }
    }
}
