using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BeltAnimConfig", menuName = "Scriptable Objects/BeltAnimConfig")]
public class BeltAnimConfig : ScriptableObject
{
    [SerializeField]
    List<AnimNames> animNameInfos = new List<AnimNames>();

    [SerializeField]
    List<AnimIndex> animIndexInfos = new List<AnimIndex>();

    private Dictionary<AnimType, string> TypeToNameMap;
    private Dictionary<string, AnimType> indexToTypeMap;
    
    public void Initialize()
    {
        if(TypeToNameMap == null)
        {
            TypeToNameMap = new();

            TypeToNameMap = animNameInfos.ToDictionary(
            value => value.animNameType,
            value => value.name
            );
        }
            

        if (indexToTypeMap == null)
        {
            indexToTypeMap = new();

            indexToTypeMap = animIndexInfos.ToDictionary(
               value => value.index,
               value => value.animIndexType
               );
        }
           
    }

    public string GetAnimName(AnimType animType)
    {
        return TypeToNameMap.TryGetValue(animType, out string animName) ? animName : "Multi";
    }

    public AnimType GetAnimType(string animIndex)
    {
        return indexToTypeMap.TryGetValue(animIndex, out var type) ? type : AnimType.None;
    }
}

[System.Serializable]
public class AnimIndex
{
    public string index = "0000";
    public AnimType animIndexType = AnimType.None;
}

[System.Serializable]
public class AnimNames
{
    public AnimType animNameType = AnimType.None;
    public string name;
}

public enum AnimType
{
    None,
    straight,
    Left,
    Right,
    Multi
}
