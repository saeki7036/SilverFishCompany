using UnityEngine;


public class HPBarTest : MonoBehaviour
{
    // HPバーの表示を管理するクラス

    [SerializeField] 
    RectTransform backGround;

    [SerializeField] 
    RectTransform HPGreenBar;

    [SerializeField]
    Vector2 offset = new Vector2(0, 100f);

    Transform target;

    float  BarScale = 1.0f;

    public void Initialize(Transform targetTransform, float percent, float barScale)
    {

        target = targetTransform;
        UpdateBar(percent);

        this.transform.localScale = target.localScale;

        BarScale = barScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector2 point = (Vector2)target.position + (offset * BarScale);

            Vector2 screenPos = (Vector2)Camera.main.WorldToScreenPoint(point);

            transform.position = screenPos;
        }
    }

    public void UpdateBar(float ratio)
    {
        HPGreenBar.anchoredPosition = new Vector2()
        { 
            x = (backGround.sizeDelta.x / 2) - (backGround.sizeDelta.x /2) * ratio,
            y = HPGreenBar.anchoredPosition.y,
        };

        HPGreenBar.sizeDelta = new Vector2()
        {
            x = backGround.sizeDelta.x * ratio,
            y = HPGreenBar.sizeDelta.y,
        };
    }
}
