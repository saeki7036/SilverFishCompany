using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TestTurret : MonoBehaviour
{
    [SerializeField]
    int interval = 150;

    [SerializeField]
    Transform FirePosTransform;

    [SerializeField]
    GameObject neckObject;

    [SerializeField]
    GameObject BulletPrehab;

    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float Range = 4f;

    [SerializeField]
    AudioClip ShotClip;

    Vector2 Target;

    int timecount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timecount = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timecount++;
        Target = EnemyManagerTest.Instance.NearestPos(transform.position, Range);

        bool SetTarget = Target == -Vector2.one;

        if (SetTarget)
        {
            neckObject.transform.up = Vector2.zero;
        }
        else
        {
            Vector2 TargetVector2 = Target - (Vector2)transform.position;

            neckObject.transform.up = TargetVector2;

            if (timecount > interval)
            {
                AudioManager.instance.isPlaySE(ShotClip);

                GameObject bullet = Instantiate(
                    BulletPrehab,
                    FirePosTransform.position, 
                    neckObject.transform.rotation);

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

                Vector2 force = TargetVector2.normalized * speed;
                rb.linearVelocity = force;

                timecount = 0;
            }
        }
        
    }
}
