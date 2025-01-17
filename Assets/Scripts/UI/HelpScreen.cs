using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : Screen
{
    private IngameUI ingameUI;
    [SerializeField] private Button backToGameButton;

    public readonly EventVariable<HelpScreen, int> selectedCategoryIndex;

    private HelpScreen()
    {
        selectedCategoryIndex = new EventVariable<HelpScreen, int>(this, 0);
    }

    private void Awake()
    {
        Hide();
        ingameUI = GetComponentInParent<IngameUI>();
        backToGameButton.onClick.AddListener(OnPress_BackToGameButton);
    }

    private void OnPress_BackToGameButton()
    {
        ingameUI.selectedScreenType.value = IngameUIScreenType.None;
    }

    private void OnDestroy()
    {
        backToGameButton.onClick.RemoveListener(OnPress_BackToGameButton);
    }
}