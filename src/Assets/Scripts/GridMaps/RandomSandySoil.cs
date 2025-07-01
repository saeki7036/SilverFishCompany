using UnityEngine;

public class RandomSandySoil : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite[] sprites;

    void Start()
    {
        // スプライトをランダムに変更する
        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }
}
