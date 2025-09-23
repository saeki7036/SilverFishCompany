using UnityEngine;
using UnityEngine.UI;
public class HPBarManagerTest : MonoBehaviour
{
    // HPバーをUIの子供に設定するため、オブジェクト参照させるクラス

    [SerializeField]
    static GameObject hpBarParent;

    public static GameObject GetParent() => hpBarParent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Awake()
    {
        hpBarParent = this.gameObject;
    }
}
