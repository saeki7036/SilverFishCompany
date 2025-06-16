using Unity.Android.Gradle.Manifest;
using UnityEditor.Rendering;
using UnityEngine;

public class ProductItem
{
    int level;
    ItemCategory category;
    GameObject itemObject;

    bool isMoveFlag;

    Vector3 moveTargetPos;
    Vector3 moveBeforePos;

    // コンストラクタ
    public ProductItem(ItemInformation information, Vector2Int CreateObjectPos)
    {
        level = information.ItemLevel;
        category = information.ItemCategory;
        isMoveFlag = false;

        Vector3 instantiatePos = new Vector3
        { 
            x = CreateObjectPos.x,
            y = CreateObjectPos.y,
            z = 0,
        };

        moveBeforePos = instantiatePos;

        itemObject = GameObject.Instantiate(information.ItemPrehab, instantiatePos, Quaternion.identity);    
    }
  
    // Getter,プロパティ
    public bool IsItemMove() => isMoveFlag;

    public ItemCategory GetCategory() => category;

    public int GetLevel() => level;

    public void AddLevel() => level++;

    public void ItemObjectDestroy()
    {
        GameObject.Destroy(itemObject);
        itemObject = null;
    }

    public void ItemMoveSetting(Vector3 targetPos)
    {
        moveTargetPos = targetPos;
        isMoveFlag = true;
    }
}
