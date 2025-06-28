using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ProductUISetting : MonoBehaviour
{
    [SerializeField]
    ProductUICreate productUICreate;

    [SerializeField]
    RectTransform ProductBackImage;

    [SerializeField]
    RectTransform SerectCursol;

    [SerializeField]
    UIContent[] ProductImages;

    static readonly float outIndexRectPosX = -2000f;

    void Start()
    {
        for (int i = 0; i < ProductImages.Length; i++)
        {
            int captureIndex = i; // forのiをローカルに

            UnityAction action = () => SetCreateProduct(captureIndex);
            ProductImages[i].SetIvent(action);
        }
    }

    void SetCreateProduct(int index)
    {

        SerectCursol.anchoredPosition = new Vector2()
        {
            x = ProductImages[index].GetRectAnchoredPosX(),
            y = SerectCursol.anchoredPosition.y
        };
        
        productUICreate.SetCreateContent(
            ProductImages[index].GetPrehab(),
            ProductImages[index].GetSprite());

        StartCoroutine(WaitResetCursol());
    }

    IEnumerator WaitResetCursol()
    {
        // 待機
        yield return new WaitUntil(() => productUICreate.IsCreated() == true);

        SerectCursol.anchoredPosition = new Vector2()
        {
            x = outIndexRectPosX,
            y = SerectCursol.anchoredPosition.y
        };

        Debug.Log("処理終了");
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
        // ② ワールド座標をスクリーン座標に変換（UI Canvas用）
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
