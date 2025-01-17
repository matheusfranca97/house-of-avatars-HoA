
using UnityEngine;
using UnityEngine.UI;

public class HelpScreenSection : MonoBehaviour
{
    private HelpScreenCategory helpScreenCategory;

    [SerializeField] private Text titleText;

    [SerializeField] private Color titleTextUnopenedColor;
    [SerializeField] private Color titleTextOpenedColor;

    [SerializeField] private Button openSectionButton;
    [SerializeField] private Button closeSectionButton;

    [SerializeField] private GameObject openedContent;

    private void Awake()
    {
        helpScreenCategory = GetComponentInParent<HelpScreenCategory>();
        helpScreenCategory.currentOpenedSection.onValueChangeImmediate += OnValueChanged_HelpScreenCategory_CurrentOpenedSection;
        openSectionButton.onClick.AddListener(OnPress_OpenSectionButton);
        closeSectionButton.onClick.AddListener(OnPress_CloseSectionButton);
    }

    private void OnValueChanged_HelpScreenCategory_CurrentOpenedSection(HelpScreenSection oldValue, HelpScreenSection newValue)
    {
        if (newValue == this)
        {
            openSectionButton.gameObject.SetActive(false);
            closeSectionButton.gameObject.SetActive(true);
            openedContent.SetActive(true);
            titleText.color = titleTextOpenedColor;
        }
        else
        {
            openSectionButton.gameObject.SetActive(true);
            closeSectionButton.gameObject.SetActive(false);
            openedContent.SetActive(false);
            titleText.color = titleTextUnopenedColor;
        }
    }

    private void OnPress_OpenSectionButton()
    {
        helpScreenCategory.currentOpenedSection.value = this;
    }

    private void OnPress_CloseSectionButton()
    {
        helpScreenCategory.currentOpenedSection.value = null;
    }

    private void OnDestroy()
    {
        helpScreenCategory.currentOpenedSection.onValueChange -= OnValueChanged_HelpScreenCategory_CurrentOpenedSection;
        openSectionButton.onClick.RemoveListener(OnPress_OpenSectionButton);
        closeSectionButton.onClick.RemoveListener(OnPress_CloseSectionButton);
    }
}