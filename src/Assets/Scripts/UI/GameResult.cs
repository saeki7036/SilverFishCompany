using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    [SerializeField]
    GamePogressManager gamePogressManager;

    [SerializeField]
    Text ResultText;

    [SerializeField]
    string ClearString = "GameClear！";

    [SerializeField]
    string OverString = "GameOver！";

    [SerializeField]
    GameObject[] objectsActivater;

    public void Start()
    {
        // タイムスケールを初期化
        Time.timeScale = 1;
    }

    /// <summary>
    /// ゲームオーバー処理(ボタン起動想定)
    /// </summary>
    public void GameOver()
    {
        gamePogressManager.SetPogressFlag(false);
        Time.timeScale = 0;
        ResultText.text = OverString;
        ObjectsActivate();
    }

    /// <summary>
    /// ゲームクリア処理(ボタン起動想定)
    /// </summary>
    public void GameClear()
    {
        gamePogressManager.SetPogressFlag(false);
        Time.timeScale = 0;
        ResultText.text = ClearString;
        ObjectsActivate();
    }

    void ObjectsActivate()
    {
        // 登録したオブジェクトをアクティブ化
        foreach (GameObject obj in objectsActivater)
        {
            obj.SetActive(true);
        }
    }
}
