using System.Collections.Generic;

using UnityEngine;

public class ProductManager : MonoBehaviour
{
    [SerializeField]
    GamePogressManager gamePogressManager;

    float addTimeCountValue = 0.02f;
    
    BuildingConfig GetConfig() => ConfigManager.Instance.GetBuildingConfig();

    float GetOperatTime(BuildType cellType) => GetConfig().GetStartupTime(cellType);

    private Dictionary<BuildType, ProductTimer> ProductOperater;

    class ProductTimer
    {
        BuildType cellType;
        float timeCount;
        float operatCount;

        public ProductTimer(BuildType cellType, float operatCount)
        {
            this.cellType = cellType;
            this.operatCount = operatCount;

            timeCount = 0f;
        }

        public BuildType GetCellType() => cellType; 

        public void AddCount(float count) => timeCount += count;

        public void CountReset() => timeCount = 0f;

        public bool IsOperat() => timeCount >= operatCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var config = GetConfig();

        addTimeCountValue = config.GetOperatCount();
           
        ProductOperater = new Dictionary<BuildType, ProductTimer>();

        foreach (BuildType type in config.GetCellTypes())
        {
            ProductOperater[type] = new ProductTimer(type,GetOperatTime(type));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gamePogressManager.GetPogressFlag())
        {
            AddTimeCount();
            OperatCheck();
        }
    }

    void AddTimeCount()
    {
        foreach (ProductTimer product in ProductOperater.Values)
        {
            product.AddCount(addTimeCountValue);
        }
    }

    void OperatCheck()
    {
        foreach (ProductTimer product in ProductOperater.Values)
        {
            if (product.IsOperat())
            {
                product.CountReset();
                GridMapManager.Instance.OperatBuilding(product.GetCellType());
            }
        }
    }
}
