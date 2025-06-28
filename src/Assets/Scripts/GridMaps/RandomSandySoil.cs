using UnityEngine;

public class RandomSandySoil : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite[] sprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }
}
