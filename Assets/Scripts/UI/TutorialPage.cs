using UnityEngine;
using UnityEngine.UI;

public class TutorialPage : Screen
{
    private TutorialScreen tutorialScreen;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        tutorialScreen = GetComponentInParent<TutorialScreen>();

        if (nextButton != null)
            nextButton.onClick.AddListener(OnPress_NextButton);

        if (backButton != null)
            backButton.onClick.AddListener(OnPress_BackButton);

        closeButton.onClick.AddListener(OnPress_CloseButton);
    }

    private void OnPress_NextButton()
    {
        tutorialScreen.tutorialPage.value++;
    }

    private void OnPress_BackButton()
    {
        tutorialScreen.tutorialPage.value--;
    }

    private void OnPress_CloseButton()
    {
        tutorialScreen.Close();
    }

    private void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(OnPress_NextButton);

        if (backButton != null)
            backButton.onClick.RemoveListener(OnPress_BackButton);

        closeButton.onClick.RemoveListener(OnPress_CloseButton);
    }
}
