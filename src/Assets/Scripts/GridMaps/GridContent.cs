using UnityEngine;

public class GridContent : MonoBehaviour
{
    [SerializeField]
    MapContent content;

    /// <summary>
    /// このオブジェクトがベルトかどうかを判定する
    /// </summary>
    /// <returns>true: ベルト, false: その他</returns>
    public bool IsBelt() => content.GridCellType == BuildType.Belt;

    // getter
    /// <summary>
    /// MapContentの読み取り専用プロパティ
    /// </summary>
    public MapContent GetContent()=> content;

    /// <summary>
    /// Transform位置をグリッドマップ範囲内の整数座標に変換する
    /// マップサイズを超えた場合は範囲内にクランプされる
    /// </summary>
    Vector2Int CrampGridPos()
    {
        // グリッドインデックスの最大値を取得（配列アクセス用に-1）
        Vector2Int GridIndexSIze = GridMapManager.Instance.MaxMapSize - Vector2Int.one;

        // Transform位置を整数に丸めてマップ範囲内にクランプ
        return new Vector2Int()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(transform.position.x), 0, GridIndexSIze.x),
            y = Mathf.Clamp(Mathf.RoundToInt(transform.position.y), 0, GridIndexSIze.y),
        };
    }

    /// <summary>
    /// GridMapManagerにコンテンツを登録する処理
    /// </summary>
    void GridSetting()
    {
        GridMapManager.Instance.StartSetContent(content);
    }

    // Transform位置からグリッド座標を計算し、GridMapManagerに登録する
    void Start()
    {
        // Transform位置をグリッド座標に変換
        Vector2Int GridPos = CrampGridPos();

        // MapContentに最小グリッド位置を設定
        content.SetMinGridPos(GridPos);

        // GridMapManagerにこのコンテンツを登録
        GridSetting();
    }
}
