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
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        gamePogressManager.SetPogressFlag(false);
        Time.timeScale = 0;
        ResultText.text = OverString;
        ObjectsActivate();
    }

    public void GameClear()
    {
        gamePogressManager.SetPogressFlag(false);
        Time.timeScale = 0;
        ResultText.text = ClearString;
        ObjectsActivate();
    }

    void ObjectsActivate()
    {
        foreach (GameObject obj in objectsActivater)
        {
            obj.SetActive(true);
        }
    }
}
