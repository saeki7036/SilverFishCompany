using UnityEngine;

public class MainGameIventManager : MonoBehaviour
{
    [SerializeField] 
    MouseController mouseController;

    [SerializeField]
    TargetCursol targetCursol;

    [SerializeField]
    CameraMovement cameraMovement;
    void Start()
    {
        targetCursol.InputRegister(mouseController);
        cameraMovement.InputRegister(mouseController);
    }
}
