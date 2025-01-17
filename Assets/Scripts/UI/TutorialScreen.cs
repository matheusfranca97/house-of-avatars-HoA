
using UnityEngine;

public class TutorialScreen : Screen
{
    private IngameUI ingameUI;
    [SerializeField] private TutorialPage[] tutorialPages;
    public readonly ControlledEventVariable<TutorialScreen, int> tutorialPage;

    private TutorialScreen()
    {
        tutorialPage = new ControlledEventVariable<TutorialScreen, int>(this, 0, Check_TutorialPage);
    }

    private int Check_TutorialPage(int value)
    {
        if (value <= 0)
            return 0;

        if (value > tutorialPages.Length - 1)
            return tutorialPages.Length - 1;

        return value;
    }

    private void Awake()
    {
        Hide();

        ingameUI = GetComponentInParent<IngameUI>();
        tutorialPage.onValueChange += OnValueChanged_TutorialPage;
        tutorialPages[tutorialPage.value].Show();
    }

    private void OnValueChanged_TutorialPage(int oldValue, int newValue)
    {
        tutorialPages[oldValue].Hide();
        tutorialPages[newValue].Show();
    }

    private void OnDestroy()
    {
        tutorialPage.onValueChange -= OnValueChanged_TutorialPage;
    }

    public void Close()
    {
        ingameUI.selectedScreenType.value = IngameUIScreenType.None;
    }
}