
using UnityEngine;
using UnityEngine.UI;

public class HelpScreenCategoryButton : MonoBehaviour
{
    private HelpScreen helpScreen;

    [SerializeField] private Button button;
    [SerializeField] private int helpIndex;

    [SerializeField] private Color selectedIndexColor;
    [SerializeField] private Color unSelectedIndexColor;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
        helpScreen = GetComponentInParent<HelpScreen>();
        helpScreen.selectedCategoryIndex.onValueChangeImmediate += OnValueChanged_HelpScreen_SelectedCategoryIndex;
    }

    private void OnValueChanged_HelpScreen_SelectedCategoryIndex(int oldValue, int newValue)
    {
        if (newValue == helpIndex)
            button.targetGraphic.color = selectedIndexColor;
        else
            button.targetGraphic.color = unSelectedIndexColor;
    }

    private void OnPress_Button()
    {
        helpScreen.selectedCategoryIndex.value = helpIndex;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}