using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] string TitleSceneName = "SR_Title";
    [SerializeField] string MainGaneSceneName = "InputTest";
    [SerializeField] float waitTime = 1.0f;

    /// <summary>
    /// 指定のシーンに遷移
    /// </summary>
    public void ChangeScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    /// <summary>
    /// 同じシーンに再度遷移させる。
    /// </summary>
    public void SceneRerode() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

    bool IsChange;

    private void Start()
    {
        IsChange = false;
        if (SceneManager.GetActiveScene().name == TitleSceneName)
            Time.timeScale = 1.0f;
    }

    /// <summary>
    /// タイトルに遷移させる。
    /// </summary>
    public void ChangeSceneTitle()
    {
        if (IsChange) return;

        if(Time.timeScale == 0)
            SceneManager.LoadSceneAsync(TitleSceneName);
        else
        StartCoroutine(WaitOneSecondCoroutine(waitTime, TitleSceneName));
    }

    /// <summary>
    /// メインゲームに遷移させる。
    /// </summary>
    public void ChangeSceneMainGame()
    {
        if (IsChange) return;

        if (Time.timeScale == 0)
            SceneManager.LoadSceneAsync(MainGaneSceneName);
        else
            StartCoroutine(WaitOneSecondCoroutine(waitTime, MainGaneSceneName));
    }

    private IEnumerator WaitOneSecondCoroutine(float waitTime, string sceneName)
    {
        Debug.Log("待機開始");
        IsChange = true;

        yield return new WaitForSeconds(waitTime);

        Debug.Log("経過！");
        IsChange = false;

        SceneManager.LoadSceneAsync(sceneName);
    }
}
