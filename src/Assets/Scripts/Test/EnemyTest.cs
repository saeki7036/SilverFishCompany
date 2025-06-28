using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public int HP = 3;
    public int ATK = 4;
    public float speed = 1.0f;
    public int Interval = 100;

    int timeCount = 0;

    [SerializeField]
    Rigidbody2D Rigidbody2D;

    public bool IsAttack;

    public warBuildTest AttackTarget;

    Vector2 currentPosition;

    public Vector2 GetCurrentPos() => currentPosition;

    public void Hit(int damege)
    {
        HP -= damege;
        if (HP <= 0)
            OnDead();
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
