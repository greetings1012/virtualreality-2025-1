using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera robotCamera;

    public void SwitchToRobotCamera()
    {
        if (mainCamera) mainCamera.gameObject.SetActive(false);
        if (robotCamera) robotCamera.gameObject.SetActive(true);
    }

    public void SwitchToMainCamera()
    {
        if (mainCamera) mainCamera.gameObject.SetActive(true);
        if (robotCamera) robotCamera.gameObject.SetActive(false);
    }
}