using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemViewer : MonoBehaviour
{
    [SerializeField]
    List<ItemView> itemViewList = new List<ItemView>();

    /// <summary>
    /// 個別のアイテム表示を管理する内部クラス
    /// カテゴリとレベルに対応するアイテム値をテキストで表示
    /// </summary>
    [System.Serializable]
    class ItemView
    {
        [SerializeField]
        int level; // アイテムのレベル

        [SerializeField]
        ItemCategory category; // アイテムカデゴリ

        [SerializeField]
        Text itemValueText; //表示用テキスト

        /// <summary>
        /// アイテム値を取得してテキストを更新
        /// </summary>
        public void UpdateText()
        {
            // ItemManagerから該当するアイテムの現在値を取得
            int Value = ItemManager.Instance.GetItemValue(category,level);
            itemValueText.text = Value.ToString();
        }
    }

    /// <summary>
    /// 毎フレーム全てのアイテム表示を更新
    /// </summary>
    void Update()
    {
        // イベント処理にする予定
        foreach (var view in itemViewList)
        {
            view.UpdateText();// アイテムビューのテキストを更新
        }
    }
}
