using System.Net;
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

    float maxTimeCount;
    float currentTimeCount;

    // コンストラクタ
    public ProductItem(ItemInformation information, Vector2Int CreateObjectPos, float MaxTimeCount)
    {
        level = information.ItemLevel;
        category = information.ItemCategory;
        maxTimeCount = MaxTimeCount;
        isMoveFlag = false;
        currentTimeCount = 0f;

        Vector3 instantiatePos = new Vector3
        { 
            x = CreateObjectPos.x,
            y = CreateObjectPos.y,
            z = 0,
        };

        moveBeforePos = instantiatePos;

        itemObject = GameObject.Instantiate(information.ItemPrehab, instantiatePos, Quaternion.identity);
        Debug.Log(itemObject);
    }
  
    // Getter,プロパティ
    public bool IsItemMove() => isMoveFlag;

    public ItemCategory GetCategory() => category;

    public bool IsEnptyItemObject() => itemObject == null;

    public int GetLevel() => level;

    public void AddLevel() => level++;

    public void SetItemEmpty() => itemObject = null;

    public void ItemMovement(float addTimeCount)
    {
        if (isMoveFlag == false)
            return;

        currentTimeCount += addTimeCount;

        float Lerptime = Mathf.Clamp01(currentTimeCount / maxTimeCount);
        itemObject.transform.position = Vector3.Lerp(moveBeforePos, moveTargetPos, Lerptime);

        if (Lerptime >= 1f)
        {
            currentTimeCount++;
            moveBeforePos = moveTargetPos;
            isMoveFlag = false;           
        }
    }

    public void ItemObjectDestroy()
    {
        GameObject.Destroy(itemObject);
        itemObject = null;
    }

    public void ItemMoveSetting(Vector3 targetPos)
    {
        if (isMoveFlag) 
            return;

        currentTimeCount = 0f;
        moveTargetPos = targetPos;
        isMoveFlag = true;
    }
}
