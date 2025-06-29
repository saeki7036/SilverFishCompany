using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BeltAnimation : MonoBehaviour
{
    [SerializeField] Animator beltAnimator;

    [SerializeField] Transform beltTransform;

    [SerializeField] BeltAnimConfig beltAnimConfig;

    // 各方向をチェックするためのベクトル
    static readonly Vector2Int[] DirectionOffset = new Vector2Int[4]
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };

    Vector2Int posVec2Int() => new Vector2Int()
    {
        x = Mathf.RoundToInt(beltTransform.position.x),
        y = Mathf.RoundToInt(beltTransform.position.y)
    };

    Quaternion GetQuaternionZ(string index)
    {
        int rotateValue = 0;

        if(index.Length != 4)
        {
            Debug.LogAssertion("範囲外のサイズ:"+ index.Length);
            return Quaternion.identity;
        }

        if (index[0] == '1')
            rotateValue = 180;
        if (index[1] == '1')
            rotateValue = 0;
        if (index[2] == '1')
            rotateValue = 270;
        if (index[3] == '1')
            rotateValue = 90;

        return Quaternion.Euler(0, 0, rotateValue);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        beltAnimConfig.Initialize();

        var beltCell = GridMapManager.Instance.GetCell(posVec2Int());

        if(beltCell.GridCellType == BuildType.Belt)
        {
            // キャスト成功時のみ実行
            if (beltCell.Building is BeltBuilding beltBuilding)
            {
                beltBuilding.SetBeltAnimation(this);
                beltBuilding.BeltAnimPlay();
            }
        }  
    }

    public void AnimationSetting(HashSet<Vector2Int> inport, HashSet<Vector2Int> export)
    {
        Vector2Int beltPosVec2Int = posVec2Int();

        string index = "";

        foreach (Vector2Int direction in DirectionOffset)
        {
            Vector2Int neighbor = beltPosVec2Int + direction;

            int value = 0;

            if (inport.Contains(neighbor)) 
                value = 1;

            if (export.Contains(neighbor)) 
                value = 2;

            index += value.ToString();
        }
        //Debug.Log(index);
        AnimType type = beltAnimConfig.GetAnimType(index);

        string animName = beltAnimConfig.GetAnimName(type);
        
        if (inport.Count == 0 || export.Count == 0 || type == AnimType.None)
        {
            return;
        }

        if (inport.Count == 1 || export.Count == 1)
        {
            beltTransform.rotation = GetQuaternionZ(index);
        }

        beltAnimator.Play(animName);
    }
}
