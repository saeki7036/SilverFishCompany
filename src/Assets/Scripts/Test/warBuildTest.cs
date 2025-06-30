using UnityEngine;
using UnityEngine.Events;

public class warBuildTest : MonoBehaviour
{
    [SerializeField]
    int HP = 100;
    
    [SerializeField]
    bool IsDestroy = true;

    [SerializeField]
    float BarScale = 0.5f;

    [SerializeField]
    public UnityEvent DestroyIvent;

    [SerializeField]
    GameObject HPbarPrehab;

    GameObject HPBar;
    HPBarTest HPBarTest;

    [SerializeField]
    GameObject HitEffect;

    [SerializeField]
    AudioClip HitClip;

    [SerializeField]
    AudioClip DieClip;

    int MaxHP;
    void Start()
    {
        MaxHP = HP;

        HPBar = Instantiate(HPbarPrehab);

        HPBar.transform.parent = HPBarManagerTest.GetParent().transform;

        HPBarTest = HPBar.GetComponent<HPBarTest>();

        HPBarTest.Initialize(this.transform, 1f, BarScale);
    }

    Vector2Int CrampGridPos()
    {
        Vector2Int GridIndexSIze = GridMapManager.Instance.MaxMapSize - Vector2Int.one;

        return new Vector2Int()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(transform.position.x), 0, GridIndexSIze.x),
            y = Mathf.Clamp(Mathf.RoundToInt(transform.position.y), 0, GridIndexSIze.y),
        };
    }

    public void Hit(int atk)
    {
        HP -= atk;

        if (!IsDestroy)
            Debug.Log("HP:"+HP);


        if(HP <= 0)
        {
            

            if (IsDestroy)
            {
                if(DieClip != null)
                    AudioManager.instance.isPlaySE(DieClip);

                GridMapManager.Instance.DestroyContent(CrampGridPos());
                Destroy(HPBar);
                Destroy(gameObject);
            }
            else
            {
                DestroyIvent?.Invoke();
            }
               
        }
        else
        {
            AudioManager.instance.isPlaySE(HitClip);
            GameObject effect = Instantiate(HitEffect,transform.position, Quaternion.identity);
            effect.transform.localScale = transform.localScale;
            HPBarTest.UpdateBar(Mathf.Clamp01((float)HP / MaxHP));
        }
            
    }
}
