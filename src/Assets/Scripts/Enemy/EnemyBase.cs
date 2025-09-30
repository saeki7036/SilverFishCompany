using System.IO;
using System.Threading;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] 
    EnemyParameter enemyParameter;

    [SerializeField] 
    ENEMY_AI AI;

    [Space,Header("基本情報")]
    [SerializeField]
    Rigidbody2D Rigidbody;

    [SerializeField]
    float BarScale = 0.7f;

    [Space,Header("演出")]
    [SerializeField]
    GameObject DieEffect;

    [SerializeField]
    AudioClip DamegeClip;

    [SerializeField]
    AudioClip DieClip;

    warBuildTest AttackTarget;

    EnemyParameter EP => enemyParameter;

    EnemyAIBase EnemyAI;

    HPBarTest HPBar;

    int AtteckCount;

    bool IsAttack;

    Vector2Int MoveTarget;

    static readonly float TargetLimit = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (EnemyAI == null)
            return;

        EnemyStart();
    }

    public void EnemyStart()
    {
        if (EnemyAI != null)
            return;

        EnemyAI = EnemyAIBase.SetAI(AI);

        EP.ParameterSetUP();

        HPBarSpawn();

        AtteckCount = 0;

        MoveTarget = EnemyAI.OutPos;

        IsAttack = false;
    }


    void HPBarSpawn()
    {
        GameObject prehab =Instantiate(HPBarManagerTest.GetPrehab());

        prehab.transform.SetParent(HPBarManagerTest.GetParent().transform, false);

        HPBar = prehab.GetComponent<HPBarTest>();

        HPBar.Initialize(this.transform, 1f, BarScale);
    }

    public void Hit(int damege)
    {
        EP.Damege(damege);

        if (EP.Death())
            OnDead();
        else
        {
            AudioManager.instance.isPlaySE(DamegeClip);
            HPBar.UpdateBar(EP.HPrate());
        }
    }

    public void EnemyUpdate()
    {
        Vector2 currentPosition = transform.position;

        if (EP.Death())
        {
            OnDead();
            return;
        }
        
        if (AttackTarget == null)
        {
            IsAttack = false;
        }

        if (IsAttack)
        {
            Rigidbody.linearVelocity = Vector3.zero;
            AtteckCount++;

            if (AtteckCount >= EP.INTERVAL)
            {
                AtteckCount = 0;
                AttackTarget?.Hit(EP.ATK);
            }
        }
        else
        {
            AtteckCount = 0;

            if (MoveTarget == EnemyAI.OutPos)
            {
                EnemyAI.SetPath(EnemyAI.GetPath(currentPosition));

                MoveTarget = EnemyAI.GetNextPoint();
            }
            else if (Vector2.Distance(currentPosition, MoveTarget) < TargetLimit)
            {
                MoveTarget = EnemyAI.GetNextPoint();
            }

            if (MoveTarget != EnemyAI.OutPos)
                Rigidbody.linearVelocity = (MoveTarget - currentPosition).normalized * EP.SPEED;

            //Rigidbody2D.AddForce((BaseCampTest.Instance.Pos - (Vector2)transform.position).normalized * speed)
        }
    }

    private void OnDead()
    {
        AudioManager.instance.isPlaySE(DieClip);
        Instantiate(DieEffect, transform.position, Quaternion.identity);

        HPBar.IsDestroy();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
        if (collision.transform.TryGetComponent<warBuildTest>(out var warBuildTest))
        {
            
            IsAttack = true;
            AttackTarget = warBuildTest;
        }
    }
}
