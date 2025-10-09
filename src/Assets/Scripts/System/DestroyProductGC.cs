using UnityEngine;
using System.Collections;

public class DestroyProductGC : MonoBehaviour
{
    [SerializeField] private float checkInterval = 30.0f; // チェック間隔（秒）

    static Transform ProductParent;

    public static Transform GetProductParent() => ProductParent;

    private void Awake()
    {
        ProductParent = transform;
    }

    void Start()
    {
        StartCoroutine(CheckAndDestroyLoop());
    }

    private IEnumerator CheckAndDestroyLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            CheckAndDestroyChildren();
        }
    }

    private void CheckAndDestroyChildren()
    {
        // 一時リストを作成して、Destroy中の参照エラーを防ぐ
        var children = new System.Collections.Generic.List<Transform>();

        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            // 子が一つもなければ削除
            if (child.childCount == 0)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
