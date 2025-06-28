using UnityEditor;
using UnityEngine;

public class GamePogressManager : MonoBehaviour
{
    bool pogressFlag;

    private void Start()
    {
        pogressFlag = true;
    }

    public void SetPogressFlag(bool flag) => pogressFlag = flag;

    public bool GetPogressFlag() => pogressFlag;
}
