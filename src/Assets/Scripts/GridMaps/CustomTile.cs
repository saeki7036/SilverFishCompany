using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// タイルマップにenumパラメータを追加する
/// </summary>
[CreateAssetMenu(menuName = "Tile/CustomTile")]
public class CustomTile :Tile
{
    public TileType tileType;
}
