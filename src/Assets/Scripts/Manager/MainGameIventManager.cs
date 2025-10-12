using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameIventManager : MonoBehaviour
{
    // 入力イベントを統括管理するマネージャークラス 入力システムを一元化している
    [SerializeField] 
    MouseController mouseController; //マウス入力を制御するコントローラー

    [SerializeField]
    TargetCursol targetCursol; //ターゲットカーソルの表示制御

    [SerializeField]
    CameraMovement cameraMovement; //カメラの移動制御

    [SerializeField]
    BeltDrawing beltDrawing; //ベルトの描画制御

    //[SerializeField]
    //ProductUISetting productUISetting; // 生産設定制御

    [SerializeField]
    ProductCreate productCreate; //建物の生成を制御

    [SerializeField]
    ProductDestroy productDestroy; //建物の破壊を制御

    void Start()
    {
        // 各機能にマウス入力を登録し、入力システムを構築

        targetCursol.InputRegister(mouseController);        // カーソル制御の入力登録
        cameraMovement.InputRegister(mouseController);      // カメラ移動の入力登録
        beltDrawing.InputRegister(mouseController);         // ベルト描画の入力登録

       　//productUISetting.InputRegister(mouseController);  // 生産UI設定の入力登録（コメントアウト）

        productCreate.InputRegister(mouseController);     // 建物生成の入力登録
        productDestroy.InputRegister(mouseController);     // 建物破壊の入力登録
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isEndGame(); // 実際のゲームを終了
        }
        if (Input.GetKeyUp(KeyCode.F4))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

    }

    void isEndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unityエディター内で再生を停止
#else
        Application.Quit(); // 実際のゲームを終了
#endif
    }
}
