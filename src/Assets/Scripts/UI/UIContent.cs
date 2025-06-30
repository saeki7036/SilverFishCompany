using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

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

    [SerializeField]
    List<ItemReqestView> itemReqestViewList;

    [System.Serializable]
    class ItemReqestView
    {
        [SerializeField]
        ItemRequest itemRequests;

        [SerializeField]
        Text ItemValueTexts;

        public ItemRequest GetRequest ()=> itemRequests;

        public void SetView()
        {
            ItemValueTexts.text = itemRequests.GetValue().ToString();
        }
    }

    void Start()
    {
        foreach (var item in itemReqestViewList)
        {
            item.SetView();
        }
    }

    public void SetEvent(UnityAction buttonAction)
    {
        Clickbutton.onClick.AddListener(buttonAction);
    }

    public bool RectContain(Vector3 screenPos) => thisRectTransform.rect.Contains(screenPos);

    public RectTransform GetRectTransform() => thisRectTransform;

    public float GetRectAnchoredPosX() => thisRectTransform.anchoredPosition.x;

    public GameObject GetPrehab() => contentPrehab;

    public Sprite GetSprite() => contentSprite;

    public List<ItemRequest> GetItemRequestList()
    {
        List<ItemRequest> requests = new List<ItemRequest>();
        foreach (var item in itemReqestViewList)
        {
            requests.Add(item.GetRequest());
        }
        return requests;
    }
}
