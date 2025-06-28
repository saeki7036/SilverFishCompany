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
        TypeToNameMap = animNameInfos.ToDictionary(value => value.animType, value => value.name);

        if(indexToTypeMap == null)
        indexToTypeMap = animIndexInfos.ToDictionary(value => value.index, value => value.animType);
    }

    public string GetAnimName(AnimType type)
    {
        return TypeToNameMap.TryGetValue(type, out string name) ? name : "Multi";
    }

    public AnimType GetAnimType(string index)
    {
        return indexToTypeMap.TryGetValue(index, out var type) ? type : AnimType.None;
    }
}

[System.Serializable]
public class AnimIndex
{
    public string index = "0000";
    public AnimType animType = AnimType.None;
}

[System.Serializable]
public class AnimNames
{
    public AnimType animType = AnimType.None;
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
