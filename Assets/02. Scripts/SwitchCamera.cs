using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject robotCamera;

    public void SwitchToRobotCamera()
    {
        if (mainCamera) mainCamera.SetActive(false);
        if (robotCamera) robotCamera.SetActive(true);
    }

    public void SwitchToMainCamera()
    {
        if (mainCamera) mainCamera.SetActive(true);
        if (robotCamera) robotCamera.SetActive(false);
    }
}