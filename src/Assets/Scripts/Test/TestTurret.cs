using UnityEngine;

public class TestTurret : MonoBehaviour
{
    // タレットの動作確認クラス
    //のち本実装に取り込む

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
        if (!GamePogressManager.GetPogressFlag())
            return;

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

                GameObject Prehab = Instantiate(
                    BulletPrehab,
                    FirePosTransform.position, 
                    neckObject.transform.rotation);

                Bullet bullet = Prehab.GetComponent<Bullet>();
                
                Vector2 force = TargetVector2.normalized * speed;

                bullet.AddVelocity(force);

                timecount = 0;
            }
        }
        
    }
}
