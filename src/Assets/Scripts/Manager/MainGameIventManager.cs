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
    void Start()
    {
        targetCursol.InputRegister(mouseController);
        cameraMovement.InputRegister(mouseController);
        beltDrawing.InputRegister(mouseController);
    }
}
