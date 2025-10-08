using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

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

    Coroutine coroutine;

    /// <summary>
    /// アイテム要求の表示を管理する内部クラス
    /// アイテム要求情報とそれに対応するUIテキストを管理
    /// </summary>
    [System.Serializable]
    class ItemReqestView
    {
        [SerializeField]
        ItemRequest itemRequests;

        [SerializeField]
        TextMeshProUGUI ItemValueTexts;

        /// <summary>
        /// アイテム要求情報を取得
        /// </summary>
        /// <returns>アイテム要求オブジェクト</returns>
        public ItemRequest GetRequest ()=> itemRequests;

        /// <summary>
        /// 要求数量をテキストに表示
        /// </summary>
        public void SetView()
        {
            ItemValueTexts.SetText(itemRequests.GetValue().ToString());
        }
    }

    void Start()
    {
        // 全てのアイテム要求ビューのテキストを設定
        foreach (var item in itemReqestViewList)
        {
            item.SetView();
        }
    }

    /// <summary>
    /// ボタンにイベントを設定
    /// </summary>
    /// <param name="buttonAction">ボタンクリック時のアクション</param>
    public void SetEvent(UnityAction buttonAction)
    {
        Clickbutton.onClick.AddListener(buttonAction);
    }

    /// <summary>
    /// 指定したスクリーン座標がこのRectTransform内に含まれるかを判定
    /// </summary>
    /// <param name="screenPos">判定するスクリーン座標</param>
    /// <returns>含まれる場合true</returns>
    public bool RectContain(Vector3 screenPos) => thisRectTransform.rect.Contains(screenPos);

    // <summary>
    /// 現在のRectTransformのスケールがDefaltの値(= 1f)かどうか判定
    /// </summary>
    /// <returns>scaleが1fならtrue</returns>
    public bool IsDefaltScale() => thisRectTransform.localScale.z == 1f;

    /// <summary>
    /// RectTransformのアンカー位置のX座標を取得
    /// </summary>
    /// <returns>アンカー位置のX座標</returns>
    public bool IsNullCoroutine() => coroutine == null;

    /// <summary>
    /// コンテンツのプレハブを取得
    /// </summary>
    /// <returns>プレハブGameObject</returns>
    public GameObject GetPrehab() => contentPrehab;

    /// <summary>
    /// コンテンツのスプライトを取得
    /// </summary>
    /// <returns>スプライト</returns>
    public Sprite GetSprite() => contentSprite;

    /// <summary>
    /// 全てのアイテム要求リストを取得
    /// </summary>
    /// <returns>アイテム要求のリスト</returns>
    public List<ItemRequest> GetItemRequestList()
    {
        List<ItemRequest> requests = new List<ItemRequest>();
        // 各アイテム要求ビューからItemRequestを抽出してリスト化
        foreach (var item in itemReqestViewList)
        {
            requests.Add(item.GetRequest());
        }
        return requests;
    }

    public void ChangeScale(float targetScale, float duration)
    {
        // 既存のスケール変更が進行中なら停止
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(CheangeScaleForCoroutine(targetScale, duration));
    }

    public IEnumerator CheangeScaleForCoroutine(float targetScale ,float duration)
    {
        Vector3 startScale = thisRectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            thisRectTransform.localScale = Vector3.Lerp(startScale, Vector3.one * targetScale, t);

            yield return null;
        }

        thisRectTransform.localScale = Vector3.one * targetScale;

        coroutine = null;
    }
}
