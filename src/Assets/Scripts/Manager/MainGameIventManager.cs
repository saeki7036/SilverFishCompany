using UnityEngine;

public class MainGameIventManager : MonoBehaviour
{
    [SerializeField] 
    MouseController mouseController;

    [SerializeField]
    TargetCursol targetCursol;

    [SerializeField]
    CameraMovement cameraMovement;

    [SerializeField]
    BeltDrawing beltDrawing;

    //[SerializeField]
    //ProductUISetting productUISetting;

    [SerializeField]
    ProductUICreate productUICreate;
    void Start()
    {
        targetCursol.InputRegister(mouseController);
        cameraMovement.InputRegister(mouseController);
        beltDrawing.InputRegister(mouseController);
        //productUISetting.InputRegister(mouseController);
        productUICreate.InputRegister(mouseController);
    }
}
