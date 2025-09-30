using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    // 敵クラス
    // のち行動パターンを複数製作予定

    [SerializeField]
    int HP = 3;

    [SerializeField]
    int ATK = 4;

    [SerializeField]
    float speed = 1.0f;

    [SerializeField]
    int Interval = 100;

    [SerializeField]
    float BarScale = 0.7f;

    [SerializeField]
    Rigidbody2D Rigidbody2D;

    [SerializeField]
    GameObject HPbarPrehab;

    [SerializeField]
    GameObject DieEffect;

    [SerializeField]
    AudioClip DamegeClip;

    [SerializeField]
    AudioClip DieClip;

    warBuildTest AttackTarget;

    Vector2 currentPosition;

    int timeCount = 0;

    int MaxHP;

    GameObject HPBar;

    HPBarTest HPBarTest;

    bool IsAttack;

    public Vector2 GetCurrentPos() => currentPosition;

    void Start()
    {
        MaxHP = HP;

        HPBar = Instantiate(HPbarPrehab);

        HPBar.transform.SetParent(HPBarManagerTest.GetParent().transform, false);
        //HPBar.transform.parent =  HPBarManagerTest.GetParent().transform;

        HPBarTest = HPBar.GetComponent<HPBarTest>();

        HPBarTest.Initialize(this.transform,1f, BarScale);
    }

    public void Hit(int damege)
    {
        HP -= damege;
        if (HP <= 0)
            OnDead();
        else
        {
            AudioManager.instance.isPlaySE(DamegeClip);
            HPBarTest.UpdateBar(Mathf.Clamp01((float)HP / MaxHP));
        }
            
    }

    public void FixedUpdates()
    {
        currentPosition = transform.position;

        if (HP <= 0)
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
            Rigidbody2D.linearVelocity = Vector3.zero;
            timeCount++;
        }
        else
        {
            timeCount = 0;
            Rigidbody2D.linearVelocity = (BaseCampTest.Instance.Pos - (Vector2)transform.position).normalized * speed;
            //Rigidbody2D.AddForce((BaseCampTest.Instance.Pos - (Vector2)transform.position).normalized * speed)
        }           

        if (timeCount >= Interval)
        {
            timeCount = 0;
            AttackTarget?.Hit(ATK);
        }
    }

    private void OnDead()
    {
        AudioManager.instance.isPlaySE(DieClip);
        Instantiate(DieEffect, transform.position, Quaternion.identity);
        Destroy(HPBar);
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
