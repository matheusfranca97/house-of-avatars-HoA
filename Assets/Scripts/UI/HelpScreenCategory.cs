
using UnityEngine;

public class HelpScreenCategory : MonoBehaviour
{
    private HelpScreen helpScreen;
    [SerializeField] private int helpScreenIndex;
    [SerializeField] private CanvasGroup canvasGroup;

    public readonly EventVariable<HelpScreenCategory, HelpScreenSection> currentOpenedSection;

    private HelpScreenCategory()
    {
        currentOpenedSection = new EventVariable<HelpScreenCategory, HelpScreenSection>(this, default);
    }

    private void Awake()
    {
        helpScreen = GetComponentInParent<HelpScreen>();
        helpScreen.selectedCategoryIndex.onValueChangeImmediate += OnValueChanged_HelpScreen_SelectedCategoryIndex;
    }

    private void OnValueChanged_HelpScreen_SelectedCategoryIndex(int oldValue, int newValue)
    {
        if (newValue == helpScreenIndex)
            canvasGroup.Show();
        else
            canvasGroup.Hide();
    }

    private void OnDestroy()
    {
        helpScreen.selectedCategoryIndex.onValueChange -= OnValueChanged_HelpScreen_SelectedCategoryIndex;
    }
}