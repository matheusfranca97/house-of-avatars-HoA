
using UnityEngine;
using UnityEngine.UI;

public class RulesScreen : Screen
{
    private IngameUI ingameUI;
    [SerializeField] private Button backToGameButton;

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