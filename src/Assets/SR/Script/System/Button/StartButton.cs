using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] SR_Sytemgame _Sytemgame;
    [SerializeField] SpriteRenderer _spriteRenderer;
    AudioManager _audioManager => AudioManager.instance;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && !_Sytemgame.Strat)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                _audioManager.isPlaySE(_Sytemgame.AudioClips[0]);
                _Sytemgame.Strat = true;
                
            }
        }

        if (_Sytemgame.Strat)
        {
            _spriteRenderer.sprite = _Sytemgame.ButtonSprites[1];
        }
        else 
        { 
            _spriteRenderer.sprite = _Sytemgame.ButtonSprites[0];
        }
    }
}
