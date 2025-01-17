
using UnityEngine;
using UnityEngine.UI;

public class SwitchViewModeButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }


    private void OnPress_Button()
    {
        if (Application.isFocused)
        {
            if (PlayerSettingsManager.instance.cameraMode.value == CameraMode.FirstPerson)
                PlayerSettingsManager.instance.cameraMode.value = CameraMode.ThirdPerson;
            else
                PlayerSettingsManager.instance.cameraMode.value = CameraMode.FirstPerson;
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}