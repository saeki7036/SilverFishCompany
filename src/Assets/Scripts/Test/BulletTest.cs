using UnityEngine;
using UnityEngine.Rendering;

public class BulletTest : MonoBehaviour
{
    [SerializeField]
    int damege = 2;

    [SerializeField]
    float DastroyTime = 6f;

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
        if(collision.transform.TryGetComponent<EnemyTest>(out var component))
        {
            component.Hit(damege);
        }

        Instantiate(EffectPrahab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
