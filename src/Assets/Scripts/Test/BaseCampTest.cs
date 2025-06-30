using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseCampTest : MonoBehaviour
{
    //public int HP = 100;

    static BaseCampTest instance;

    [HideInInspector]
    public static BaseCampTest Instance => instance;

    public Vector2 Pos;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 複数生成を防止
            return;
        }

        instance = this;

        Pos = transform.position;
    }
}
