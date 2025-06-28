using System.Collections.Generic;
using UnityEngine;

public class SR_Sytemgame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool Strat = false;
    [SerializeField] Animator StarAnimator;
    
    public List<AudioClip> AudioClips = new List<AudioClip>();

    public List<Sprite> ButtonSprites = new List<Sprite>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Strat) 
        {
            StarAnimator.Play("ˆÃ“]");
        }
    }
}
