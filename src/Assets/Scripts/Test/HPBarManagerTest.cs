using UnityEngine;
using UnityEngine.UI;
public class HPBarManagerTest : MonoBehaviour
{
    [SerializeField]
    static GameObject hpBarParent;

    public static GameObject GetParent() => hpBarParent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Awake()
    {
        hpBarParent = this.gameObject;
    }
}
