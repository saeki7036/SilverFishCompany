using System.Collections.Generic;
using UnityEngine;

public class BombBullet : Bullet
{
    [Header("BombBullet")]
    [Space]
    [SerializeField]
    float BombExprodeRange;

    protected override void TriggerAction(Collider2D collision = null)
    {
        if (collision.transform.TryGetComponent<EnemyBase>(out var component))
        {
            List<EnemyBase> list = EnemyManagerTest.Instance.NearEnemyList(collision.transform.position, BombExprodeRange);

            list.Add(component);

            foreach (var enemy in list)
            {
                enemy.Hit(damege);
            }
        }

        Instantiate(EffectPrahab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // ギズモの色を赤に設定
        Gizmos.DrawWireSphere(transform.position, BombExprodeRange); // 球体を描画
    }
}
