using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ProductUISetting : MonoBehaviour
{
    [SerializeField]
    ProductUICreate productUICreate; // 生成処理を行うクラス

    [SerializeField]
    RectTransform SerectCursol;// 選択中の建物に表示されるカーソル

    [SerializeField]
    float adjustmentX = -65f;// カーソル位置の補正値（デザインに合わせて位置調整）

    [SerializeField]
    UIContent[] ProductImages;// 建物のUIコンテンツ配列

    const float outIndexRectPosX = -2000f;// 非表示にするためのX座標（画面外）

    void Start()
    {
        // 各UIContentにクリックイベントを登録する
        for (int i = 0; i < ProductImages.Length; i++)
        {
            int captureIndex = i; // forのiをローカルに

            // UIContent 側にイベント登録
            UnityAction action = () => SetCreateProduct(captureIndex);
            ProductImages[i].SetEvent(action);
        }
    }

    /// <summary>
    /// 指定したインデックスのUIを生成し、カーソルを移動させる
    /// </summary>
    /// <param name="index">選択されたUIのインデックス</param>
    void SetCreateProduct(int index)
    {
        // 選択カーソルの位置を調整して移動
        SerectCursol.anchoredPosition = new Vector2()
        {
            x = ProductImages[index].GetRectAnchoredPosX() - adjustmentX,
            y = SerectCursol.anchoredPosition.y
        };

        // UIからの生成指示
        productUICreate.SetCreateContent(
            ProductImages[index].GetItemRequestList(),
            ProductImages[index].GetPrehab(),
            ProductImages[index].GetSprite());

        // 生成が終わったらカーソルを非表示に戻す処理を開始
        StartCoroutine(WaitResetCursol());
    }

    /// <summary>
    /// UI生成が完了するまで待機し、選択カーソルを画面外へ戻す
    /// </summary>
    IEnumerator WaitResetCursol()
    {
        // 待機
        yield return new WaitUntil(() => productUICreate.IsCreated() == true);

        SerectCursol.anchoredPosition = new Vector2()
        {
            x = outIndexRectPosX,
            y = SerectCursol.anchoredPosition.y
        };

        // Debug.Log("処理終了");
    }

    /*
    public void InputRegister(MouseController input)
    {
        input.LeftUpEvent += CheckProductImages;
    }

    bool IsScreenPointInsideRect(RectTransform rectTransform, Vector3 screenPos)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Debug.Log(screenPos + ":" + corners[0] + ":" + corners[2]);

        return screenPos.x >= corners[0].x &&
               screenPos.x <= corners[2].x &&
               screenPos.y >= corners[0].y &&
               screenPos.y <= corners[2].y;
    }

    int IsInBackImage(Vector3 screenPos)
    {
        for(int i = 0; i < ProductImages.Length;i++)
        {
            Debug.Log(IsScreenPointInsideRect(ProductImages[i].GetRectTransform(), screenPos));
            if(IsScreenPointInsideRect(ProductImages[i].GetRectTransform(), screenPos))
                return i;

            if (RectTransformUtility.RectangleContainsScreenPoint(
                ProductImages[i].GetRectTransform(), screenPos, Camera.main))
                return i;
        }

        return outIndex;
    }

    Vector2 AnchoredPositionSetting() => new Vector2()
    {
        x = currentIndex == outIndex ? outIndexRectPosX : ProductImages[currentIndex].GetRectAnchoredPosX(),
        y = 0
    };

    void CheckProductImages(Vector3 mouseWorldUpPos)
    {
        if (ProductImages.Length == 0 || false)
            return;

        // スクリーン座標に変換
        // ワールド座標をスクリーン座標に変換（UI Canvas用）
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, mouseWorldUpPos);

        //Vector3 screenPos = Camera.main.WorldToScreenPoint(mouseWorldUpPos);

        //Debug.Log(RectTransformUtility.RectangleContainsScreenPoint(ProductBackImage, screenPos, Camera.main));

        if (!RectTransformUtility.RectangleContainsScreenPoint(ProductBackImage, screenPos, Camera.main))
            return;

        currentIndex = IsInBackImage(screenPos);
        Debug.Log(screenPos);
        SerectCursol.anchoredPosition = AnchoredPositionSetting();

        if (currentIndex == outIndex)
            productUICreate.SetCreateContent(null, null);
        else
            productUICreate.SetCreateContent(ProductImages[currentIndex].GetPrehab(), ProductImages[currentIndex].GetSprite());
    }
    */
}
