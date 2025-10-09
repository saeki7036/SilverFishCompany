using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ProductUI : MonoBehaviour
{
    [Header("ProductUI")]
    [SerializeField]
    RectTransform thisRectTransform;

    [SerializeField]
    Button Clickbutton;

    Coroutine coroutine;

    /// <summary>
    /// 現在のコルーチンがNullか判定
    /// </summary>
    /// <returns>null なら true</returns>
    public bool IsNullCoroutine() => coroutine == null;

    /// <summary>
    /// ボタンにイベントを設定
    /// </summary>
    /// <param name="buttonAction">ボタンクリック時のアクション</param>
    public void SetEvent(UnityAction buttonAction)
    {
        Clickbutton.onClick.AddListener(buttonAction);
    }

    /// <summary>
    /// スケール変更する非同期処理を実行する
    /// </summary>
    /// <param name="targetScale">目標スケール</param>
    /// <param name="duration">遷移時間</param>
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

    IEnumerator CheangeScaleForCoroutine(float targetScale, float duration)
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
