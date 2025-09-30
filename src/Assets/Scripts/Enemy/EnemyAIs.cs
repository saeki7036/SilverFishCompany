using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public enum ENEMY_AI
{
    None,
    Soldier,
    Fry,
}

public class EnemyAIBase
{
    List<Vector2Int> Path;

    public readonly Vector2Int OutPos = new Vector2Int(-999, -999);

    public bool IsNullPath => Path == null;

    public void SetPath(List<Vector2Int> path) => Path = path;

    public bool IsOutPos (Vector2Int vector2Int) => OutPos.Equals(vector2Int);

    public Vector2Int GetNextPoint()
    {
        if (Path == null || Path.Count <= 0)
            return OutPos;

        Vector2Int point = Path.First();

        Path.RemoveAt(0);

        return point;
    }

    public static EnemyAIBase SetAI(ENEMY_AI ai)
    {
        if(ai == ENEMY_AI.Soldier)
        {
            return new EnemyAISoldier();
        }

        if(ai == ENEMY_AI.Fry)
        {

        }

        return new EnemyAIBase();
    }

    public virtual List<Vector2Int> GetPath(Vector2 EnemyPosition)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        Vector2 goal = EnemyManagerTest.Instance.GetBaseCampPosSenter();

        //マップサイズ
        Vector2Int mapSize = GridMapManager.Instance.MaxMapSize - Vector2Int.one;

        // 開始とゴール
        // 開始
        Vector2Int goal2D =　new Vector2Int()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(goal.x), 0, mapSize.x),
            y = Mathf.Clamp(Mathf.RoundToInt(goal.y), 0, mapSize.y),
        };

        path .Add(goal2D);

        return path;
    }
}
