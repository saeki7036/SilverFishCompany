using System.Collections.Generic;

using UnityEngine;

public class ProductManager : MonoBehaviour
{
    float addTimeCountValue = 0.02f;
    bool AddTimeCountFlag = true;

    BuildingConfig GetConfig() => ConfigManager.Instance.GetBuildingConfig();

    float GetOperatTime(CellType cellType) => GetConfig().GetStartupTime(cellType);

    private Dictionary<CellType, ProductTimer> ProductOperater;

    class ProductTimer
    {
        CellType cellType;
        float timeCount;
        float operatCount;

        public ProductTimer(CellType cellType, float operatCount)
        {
            this.cellType = cellType;
            this.operatCount = operatCount;

            timeCount = 0f;
        }

        public CellType GetCellType() => cellType; 

        public void AddCount(float count) => timeCount += count;

        public void CountReset() => timeCount = 0f;

        public bool IsOperat() => timeCount >= operatCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var config = GetConfig();

        addTimeCountValue = config.GetOperatCount();
           
        ProductOperater = new Dictionary<CellType, ProductTimer>();

        foreach (CellType type in config.GetCellTypes())
        {
            ProductOperater[type] = new ProductTimer(type,GetOperatTime(type));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!AddTimeCountFlag) 
            return;

        AddTimeCount();
        OperatCheck();
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
