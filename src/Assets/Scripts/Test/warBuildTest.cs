using UnityEngine;
using UnityEngine.Events;

public class warBuildTest : MonoBehaviour
{
    public int HP = 100;

    public bool IsDestroy = true;

    [SerializeField]
    public UnityEvent DestroyIvent;

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
        if(HP <= 0)
        {
            DestroyIvent?.Invoke();

            if (IsDestroy)
            {
                GridMapManager.Instance.DestroyContent(CrampGridPos());
                Destroy(gameObject);
            }
               
        }
    }
}
