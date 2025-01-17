
using UnityEngine;

public class StartupScreen : Screen
{
    public StartupUI startupUI { private set; get; }
    [SerializeField] private StartupScreenType startupScreenType;

    protected virtual void Awake()
    {
        Hide();
        startupUI = GetComponentInParent<StartupUI>();
        startupUI.selectedScreenType.onValueChangeImmediate += OnValueChanged_StartupUI_SelectedScreenType;
    }

    private void OnValueChanged_StartupUI_SelectedScreenType(StartupScreenType oldValue, StartupScreenType newValue)
    {
        if (oldValue == startupScreenType)
            Hide();

        if (newValue == startupScreenType)
            Show();
    }

    private void OnDestroy()
    {
        startupUI.selectedScreenType.onValueChange -= OnValueChanged_StartupUI_SelectedScreenType;
    }
}