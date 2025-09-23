using UnityEditor;
using UnityEngine;

public class GamePogressManager : MonoBehaviour
{
    // ゲームの進行可否を制御するフラグの設定・取得を行うマネージャークラス

    /// <summary>
    /// ゲーム進行フラグ
    /// true: ゲーム進行可能, false: ゲーム進行停止
    /// </summary>
    bool pogressFlag;

    private void Start()
    {
        // ゲーム開始時に進行フラグをtrueで初期化
        pogressFlag = true;
    }

    /// <summary>
    /// ゲーム進行フラグを設定
    /// </summary>
    /// <param name="flag">設定するフラグ値 (true: 進行可能, false: 進行停止)</param>
    public void SetPogressFlag(bool flag) => pogressFlag = flag;

    /// <summary>
    /// ゲーム進行フラグを取得
    /// </summary>
    /// <returns>現在の進行フラグ値 (true: 進行可能, false: 進行停止)</returns>
    public bool GetPogressFlag() => pogressFlag;
}
