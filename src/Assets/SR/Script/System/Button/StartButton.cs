using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] SR_Sytemgame _Sytemgame;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] BoxCollider2D _boxCollider;
    AudioManager _audioManager => AudioManager.instance;

    public enum ButtonType 
    { 
    
        Start,
        Exit

    }
    public ButtonType buttonType = ButtonType.Start;
   
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

            if (hit.collider != null && hit.collider == _boxCollider && !_Sytemgame.Strat &&!_Sytemgame.Exit)
            {
                _audioManager.isPlaySE(_Sytemgame.AudioClips[0]);
                if (buttonType == ButtonType.Start)
                {
                    _Sytemgame.Strat = true;
                }
                else if (buttonType == ButtonType.Exit) 
                {
                    _Sytemgame.Exit = true;
                }
            }
        }

        switch (buttonType) 
        {
        
            case ButtonType.Start:
                isStartButton();
                break; case ButtonType.Exit:
                isExitButton();
                break;

        }

        
    }

    public void isStartButton() 
    { 
    if (_Sytemgame.Strat)
        {
            _spriteRenderer.sprite = _Sytemgame.ButtonSprites[1];
        }
        else 
        { 
            _spriteRenderer.sprite = _Sytemgame.ButtonSprites[0];
        }
    }
    public void isExitButton() 
    {
        if (_Sytemgame.Exit)
        {
            _spriteRenderer.sprite = _Sytemgame.ButtonSprites[3];
        }
        else
        {
            _spriteRenderer.sprite = _Sytemgame.ButtonSprites[2];
        }
    }
}
