using UnityEngine;
using UnityEngine.Rendering;

public class BulletTest : MonoBehaviour
{
    public int damege = 2;
    public float DastroyTime = 6f;
    void Start()
    {
        Destroy(gameObject, DastroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.TryGetComponent<EnemyTest>(out var component))
        {
            component.Hit(damege);
        }

        Destroy(gameObject);
    }
}
