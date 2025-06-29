using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class HPBarTest : MonoBehaviour
{
    [SerializeField] Image fillImage;

    Transform target;
    Vector2 offset = new Vector2(0, 1f);

    public void Initialize(Transform targetTransform, float percent)
    {
        target = targetTransform;
        UpdateBar(percent);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector2 point = (Vector2)target.position + offset;

            Vector2 screenPos = Camera.main.WorldToScreenPoint(point);

            transform.position = screenPos;
        }
    }

    void UpdateBar(float ratio)
    {
        fillImage.fillAmount = ratio;
    }
}
