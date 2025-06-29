using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemViewer : MonoBehaviour
{
    [SerializeField]
    List<ItemView> itemViewList = new List<ItemView>();

    [System.Serializable]
    class ItemView
    {
        [SerializeField]
        int level;

        [SerializeField]
        ItemCategory category;
       
        [SerializeField]
        Text itemValueText;
            
        public void UpdateText()
        {
            int Value = ItemManager.Instance.GetItemValue(category,level);
            itemValueText.text = Value.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var view in itemViewList)
        {
            view.UpdateText();
        }
    }
}
