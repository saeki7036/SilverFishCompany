using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    //　弾クラス

    [SerializeField]
    int damege = 2;//ダメージ

    [SerializeField]
    float DastroyTime = 6f;//消えるまでの時間

    [SerializeField]
    GameObject BulletLinePrehab;

    [SerializeField]
    GameObject EffectPrahab;

    [SerializeField]
    Rigidbody2D rb2D;

    Vector2 addVelocity;
    float DestroyCount;

    public void AddVelocity(Vector2 velocity)
    {
        addVelocity = velocity;  
    }


    void Start()
    {
        Instantiate(BulletLinePrehab, transform);
    }

    private void FixedUpdate()
    {
        if (!GamePogressManager.GetPogressFlag())
            rb2D.linearVelocity = Vector2.zero;
        else
        {
            rb2D.linearVelocity = addVelocity;

            DestroyCount += Time.fixedDeltaTime;
            
            if(DestroyCount >= DastroyTime)
            {
                Destroy(gameObject);
            }
        }          
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.TryGetComponent<EnemyBase>(out var component))
        {
            component.Hit(damege);
        }

        Instantiate(EffectPrahab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
