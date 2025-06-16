using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    [SerializeField]
    BuildingTimeConfig buildingTime;

    [SerializeField]
    float addTimeCountValue = 0.02f;

    bool AddTimeCountFlag = true;

    float GetOperatTime(CellType cellType) => buildingTime.GetStartupTime(cellType);

    private Dictionary<CellType, ProductTimer> ProductOperater;

    public class ProductTimer
    {
        public CellType cellType;
        public float timeCount;

        public bool IsOperat(float operatTime) => timeCount >= operatTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ProductOperater = new Dictionary<CellType, ProductTimer>();

        foreach (var config in buildingTime.startupConfigs)
        {
            ProductOperater[config.type] = new ProductTimer
            {
                cellType = config.type,
                timeCount = 0,
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        AddTimeCount();
        OperatCheck();
    }

    void AddTimeCount()
    {
        if (!AddTimeCountFlag) return;

        foreach (ProductTimer product in ProductOperater.Values)
        {
            product.timeCount += addTimeCountValue;
        }
    }

    void OperatCheck()
    {
        foreach (ProductTimer product in ProductOperater.Values)
        {
            if (product.IsOperat(GetOperatTime(product.cellType)))
            {
                product.timeCount = 0f;
                GridMapManager.Instance.OperatBuilding(product.cellType);
            }
        }
    }
}
