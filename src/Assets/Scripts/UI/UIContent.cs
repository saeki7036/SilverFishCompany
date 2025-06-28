using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIContent : MonoBehaviour
{
    [SerializeField]
    Button Clickbutton;

    [SerializeField]
    RectTransform thisRectTransform;

    [SerializeField]
    GameObject contentPrehab;

    [SerializeField]
    Sprite contentSprite;

    public void SetIvent(UnityAction buttonAction)
    {
        Clickbutton.onClick.AddListener(buttonAction);
    }

    public bool RectContain(Vector3 screenPos) => thisRectTransform.rect.Contains(screenPos);

    public RectTransform GetRectTransform() => thisRectTransform;

    public float GetRectAnchoredPosX() => thisRectTransform.anchoredPosition.x;

    public GameObject GetPrehab() => contentPrehab;

    public Sprite GetSprite() => contentSprite;
}
