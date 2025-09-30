using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar 
{
    private static readonly float rute2 = 1.4142f;

    // ノード情報
    private class Node
    {
        public Vector2Int Pos;
        public float G; // 開始からのコスト
        public float H; // ゴールまでの推定コスト
        public float F => G + H;
        public Node Parent;
    }

    // 方向ベクトル（8方向）
    private static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // 上
        new Vector2Int(1, 0),   // 右
        new Vector2Int(0, -1),  // 下
        new Vector2Int(-1, 0),  // 左
        new Vector2Int(1, 1),   // 右上
        new Vector2Int(1, -1),  // 右下
        new Vector2Int(-1, -1), // 左下
        new Vector2Int(-1, 1)   // 左上
    };

    private static Vector2 GoalPosSenter(HashSet<Vector2Int> goals)
    {
        Vector2 senter = Vector2Int.zero;

        foreach(Vector2Int vector2Int in goals)
        {
            senter += vector2Int;
        }

        return senter / goals.Count;
    }

    /// <summary>
    /// A*で経路探索を行う
    /// </summary>
    /// <param name="start">開始位置</param>
    /// <param name="goal">ゴール位置</param>
    /// <param name="isWalkable">座標が移動可能かを判定するラムダ式</param>
    /// <param name="getMapSize">マップサイズを返すラムダ式 (幅, 高さ)</param>
    /// <returns>経路のリスト (ゴールまでたどれなければ空リスト)</returns>
    public static List<Vector2Int> FindPath(
        Vector2Int start,
        HashSet<Vector2Int> goals,
        Func<Vector2Int, bool> isWalkable,
        Func<Vector2Int> getMapSize)
    {
        var openList = new List<Node>();
        var closedList = new HashSet<Vector2Int>();
        var goalPosSenter = GoalPosSenter(goals);
        var startNode = new Node { Pos = start, G = 0, H = Heuristic(start, goalPosSenter) };
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // F値が最小のノードを選択
            openList.Sort((a, b) => a.F.CompareTo(b.F));
            var current = openList[0];

            if (goals.Contains(current.Pos))
            {
                return ReconstructPath(current);
            }

            openList.RemoveAt(0);
            closedList.Add(current.Pos);

            foreach (var dir in Directions)
            {
                var nextPos = current.Pos + dir;

                // マップ範囲外チェック
                Vector2Int size = getMapSize();
                if (nextPos.x < 0 || nextPos.y < 0 || nextPos.x >= size.x || nextPos.y >= size.y)
                    continue;

                // 移動不可チェック
                if (!isWalkable(nextPos))
                    continue;

                // 斜め移動時の角抜け禁止
                if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) == 2)
                {
                    Vector2Int side1 = new Vector2Int(current.Pos.x + dir.x, current.Pos.y);
                    Vector2Int side2 = new Vector2Int(current.Pos.x, current.Pos.y + dir.y);

                    if (!isWalkable(side1) || !isWalkable(side2))
                        continue;
                }

                if (closedList.Contains(nextPos))
                    continue;

                float tentativeG = current.G + ((dir.x == 0 || dir.y == 0) ? 1f : rute2); // 直交:1, 斜め:√2

                Node existing = openList.Find(n => n.Pos == nextPos);
                if (existing == null)
                {
                    var nextNode = new Node
                    {
                        Pos = nextPos,
                        G = tentativeG,
                        H = Heuristic(nextPos, goalPosSenter),
                        Parent = current
                    };
                    openList.Add(nextNode);
                }
                else if (tentativeG < existing.G)
                {
                    existing.G = tentativeG;
                    existing.Parent = current;
                }
            }
        }

        // 経路が見つからなかった場合
        return new List<Vector2Int>();
    }

    // ヒューリスティック(マンハッタン距離 + 斜め考慮)
    private static float Heuristic(Vector2 a, Vector2 b)
    {
        float dx = Mathf.Abs(a.x - b.x);
        float dy = Mathf.Abs(a.y - b.y);
        return (dx + dy) + (rute2 - 2) * Mathf.Min(dx, dy);
    }

    // 経路復元
    private static List<Vector2Int> ReconstructPath(Node node)
    {
        var path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.Pos);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
}
