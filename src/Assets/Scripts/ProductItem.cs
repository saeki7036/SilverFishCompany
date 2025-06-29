using System.Net;
//using Unity.Android.Gradle.Manifest;
using UnityEditor.Rendering;
using UnityEngine;

public class ProductItem
{
    int level;
    ItemCategory category;
    GameObject itemObject;

    Vector3 moveTargetPos;
    Vector3 moveBeforePos;

    float maxTimeCount;
    float currentTimeCount;

    bool isMoveFlag;
    bool isUpdateFlag;

    ItemInformation nextLevelInfomation;

    // コンストラクタ
    public ProductItem(ItemInformation information, Vector2Int CreateObjectPos, float MaxTimeCount)
    {
        level = information.GetItemLevel();
        category = information.GetItemCategory();
        maxTimeCount = MaxTimeCount;
        isMoveFlag = false;
        isUpdateFlag = false;
        currentTimeCount = 0f;

        Vector3 instantiatePos = new Vector3
        { 
            x = CreateObjectPos.x,
            y = CreateObjectPos.y,
            z = 0,
        };

        moveBeforePos = instantiatePos;

        itemObject = GameObject.Instantiate(information.GetItemPrehab(), instantiatePos, Quaternion.identity);

        //Debug.Log(itemObject);

        nextLevelInfomation = null;
    }
  
    // Getter,プロパティ
    
    public ItemCategory GetCategory() => category;

    public int GetLevel() => level;
  
    public void SetNextLevelInfo(ItemInformation info) => nextLevelInfomation = info; 

    public bool IsItemMove() => isMoveFlag;

    public bool IsItemUpdate() => isUpdateFlag;

    public bool IsSetNextLevelInfo() => nextLevelInfomation != null;

    public bool IsEnptyItemObject() => itemObject == null;

    public bool IsSameCategoryAndNearLevel(ItemInformation itemInformation, int difference = 1)
    {
        if (itemInformation == null)
           return false;
       
        if (itemInformation.GetItemCategory() != category)
            return false;

        //Debug.Log(itemInformation.GetItemLevel()+"=="+ level + "+" + difference);
        return itemInformation.GetItemLevel() != level + difference;
    }
    
    public void ItemMovement(float addTimeCount)
    {
        if (isMoveFlag == false)
            return;

        currentTimeCount += addTimeCount;

        float Lerptime = Mathf.Clamp01(currentTimeCount / maxTimeCount);

        itemObject.transform.position = Vector3.Lerp(moveBeforePos, moveTargetPos, Lerptime);

        if (Lerptime >= 1f)
        {
            moveBeforePos = moveTargetPos;
            
            if (nextLevelInfomation == null)
                isMoveFlag = false;
            else
                isUpdateFlag = true;
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

    public void NextLevelSetting()
    {
        Vector3 itemPos = itemObject.transform.position;

        ItemObjectDestroy();

        level = nextLevelInfomation.GetItemLevel();

        itemObject = GameObject.Instantiate(nextLevelInfomation.GetItemPrehab(), itemPos, Quaternion.identity);

        nextLevelInfomation = null;

        isMoveFlag = false;
        isUpdateFlag = false;
    }
}
