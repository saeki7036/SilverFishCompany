using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SR_Sytemgame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool Strat = false;
    public bool Exit = false;
    public bool Rock = false;
    [SerializeField] Animator StarAnimator;
    float WaitCount = 0;
    
    public List<AudioClip> AudioClips = new List<AudioClip>();

    public List<Sprite> ButtonSprites = new List<Sprite>();

    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Strat || Exit) 
        {
            StarAnimator.Play("暗転");
            WaitCount += Time.deltaTime;
            if (WaitCount > 2 && !Rock)
            {
                Rock = true;

                if (Strat)
                {
                    isStartGame();
                }
                else if (Exit) 
                {
                    isEndGame();
                }
                
            }
        }
    }

    public void isStartGame() 
    {
        SceneManager.LoadScene("InputTest");
    }

    public void isEndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unityエディター内で再生を停止
#else
        Application.Quit(); // 実際のゲームを終了
#endif
    }
}
