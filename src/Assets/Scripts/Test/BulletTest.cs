using UnityEngine;
using UnityEngine.Rendering;

public class BulletTest : MonoBehaviour
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

    void Start()
    {
        Destroy(gameObject, DastroyTime);

        Instantiate(BulletLinePrehab, transform);
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
