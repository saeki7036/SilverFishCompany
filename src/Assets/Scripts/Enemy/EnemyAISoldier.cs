using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyAISoldier : EnemyAIBase
{
    bool IsWalkable(Vector2Int visitPos)
    {
        BuildType buildType = GridMapManager.Instance.GetCell(visitPos).GridCellType;

        return buildType != BuildType.Wall;
    }

    public override List<Vector2Int> GetPath(Vector2 EnemyPosition)
    {
        // ラムダ定義
        // 移動可能か判断する関数
        Func<Vector2Int, bool> isWalkable = pos => IsWalkable(pos);

        // マップサイズ
        Func<Vector2Int> mapSize = () => GridMapManager.Instance.MaxMapSize - Vector2Int.one;

        // 開始とゴール
        // 開始
        Vector2Int start = new Vector2Int()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(EnemyPosition.x), 0, mapSize().x),
            y = Mathf.Clamp(Mathf.RoundToInt(EnemyPosition.y), 0, mapSize().y),
        };

        // ゴール
        HashSet<Vector2Int> goals = new HashSet<Vector2Int>();
        goals = EnemyManagerTest.Instance.GetBaseCampPos();

        // 経路探索(Astar)
        List<Vector2Int> path = AStar.FindPath(start, goals, isWalkable, mapSize);

        // 経路探索が失敗した時に、基底クラスでの探索に移行
        if(path == new List<Vector2Int>())
            return base.GetPath(EnemyPosition);

        /*
        foreach (var p in path)
        {
            Debug.Log($"Path: {p}");
        }
        */

        return path;
    }
}
