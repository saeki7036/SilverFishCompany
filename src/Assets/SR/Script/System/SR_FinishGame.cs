using UnityEngine;

public class SR_FinishGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Unityエディター内で再生を停止
#else
        Application.Quit(); // 実際のゲームを終了
#endif
        }
    }
}
