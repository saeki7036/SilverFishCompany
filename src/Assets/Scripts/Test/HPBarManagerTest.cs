using UnityEngine;
using UnityEngine.UI;
public class HPBarManagerTest : MonoBehaviour
{
    // HPバーをUIの子供に設定するため、オブジェクト参照させるクラス

    [SerializeField] GameObject HpBarPrehab;

    [SerializeField] static GameObject HpBarParent;

    static GameObject hpBarPrehab;

    public static GameObject GetPrehab() => hpBarPrehab;

    public static GameObject GetParent() => HpBarParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hpBarPrehab = HpBarPrehab;
        HpBarParent = this.gameObject;
    }
}
