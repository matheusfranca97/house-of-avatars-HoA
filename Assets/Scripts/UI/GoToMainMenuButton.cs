
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GoToMainMenuButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private StartupScreenType startupScreenType;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        if (PlayerController.instance != null && PlayerController.instance.sittingInteractable != null)
        {
            PlayerController.instance.sittingInteractable.ServerUnsetSittableRPC(NetworkManager.Singleton.LocalClientId);
        }
        SupabaseBridge.instance.LeaveServer();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}