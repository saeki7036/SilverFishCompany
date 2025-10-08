using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ProductUISetting : MonoBehaviour
{
    [SerializeField]
    ProductUICreate productUICreate; // 生成処理を行うクラス

    [SerializeField]
    UIContent[] ProductImages;// 建物のUIコンテンツ配列

    [SerializeField]
    float ScaleUpTime = 0.3f;

    [SerializeField]
    float ScaleDownTime = 0.3f;

    [SerializeField]
    float DefaultScale = 1.0f;

    [SerializeField]
    float SelectScale = 1.45f;

    int currentIndex = -1;//選択しているインデックス
    Coroutine CurrentCoroutine; // 実行中のコルーチン

    public void IndexReset() => currentIndex = -1;

    void Start()
    {
        currentIndex = -1;

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
    /// 指定したインデックスのUIを選択し、UIを拡大させる
    /// </summary>
    /// <param name="index">選択されたUIのインデックス</param>
    void SetCreateProduct(int index)
    {
        productUICreate.CancelCreate();

        if (currentIndex == index)
        {
            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
            }

            CurrentCoroutine = StartCoroutine(WaitResetScale(currentIndex));
        }
        else
        {
            if(CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);

                if (currentIndex != -1)
                    ProductImages[currentIndex].ChangeScale(DefaultScale, ScaleDownTime);
            }

            // UIのスケールを拡大
            ProductImages[index].ChangeScale(SelectScale, ScaleUpTime);

            // UIからの生成指示
            productUICreate.SetCreateContent(
                ProductImages[index].GetItemRequestList(),
                ProductImages[index].GetPrehab(),
                ProductImages[index].GetSprite());

            // 生成が終わったらカーソルを非表示に戻す処理を開始
            CurrentCoroutine = StartCoroutine(WaitResetCursol(index));

            currentIndex = index;
        }

        return;
    }

    /// <summary>
    /// 建物の生成が完了するまで待機し、選択したUIを元のサイズへ戻す
    /// </summary>
    IEnumerator WaitResetCursol(int index)
    {
        // 待機
        yield return new WaitUntil(() => productUICreate.IsCreated() == true);

        // UIのスケールを戻す
        ProductImages[index].ChangeScale(DefaultScale, ScaleDownTime);
       
        currentIndex = -1;

        CurrentCoroutine = null;
    }

    /// <summary>
    /// 同じUIを選択した時に、元のサイズへ戻す動作を待機する
    /// </summary>
    IEnumerator WaitResetScale(int index)
    {
        // UIのスケールを戻す
        ProductImages[index].ChangeScale(DefaultScale, ScaleDownTime);
        // 待機
        yield return new WaitUntil(() => ProductImages[index].IsNullCoroutine() == true);

        currentIndex = -1;

        CurrentCoroutine = null;
    }

    /*
       // 同じUIを選んだ場合キャンセル処理
       if (!ProductImages[index].IsDefaltScale())
       {
           currentIndex = -1;
           createdIndex = -1;
           productUICreate.CancelCreate();
           return;
       }

       Debug.Log("qqq");

       currentIndex = index;

       // UIのスケールを拡大
       ProductImages[index].ChangeScale(SelectScale, ScaleUpTime);

       // 選択カーソルの位置を調整して移動

       SerectCursol.anchoredPosition = new Vector2()
       {
           x = ProductImages[index].GetRectAnchoredPosX() - adjustmentX,
           y = SerectCursol.anchoredPosition.y
       };*/

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
