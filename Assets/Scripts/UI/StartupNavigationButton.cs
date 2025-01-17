
using UnityEngine;
using UnityEngine.UI;

public class StartupNavigationButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private StartupScreenType startupScreenType;

    private StartupUI startupUI;

    private void Awake()
    {
        startupUI = GetComponentInParent<StartupUI>();
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        startupUI.selectedScreenType.value = startupScreenType;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}