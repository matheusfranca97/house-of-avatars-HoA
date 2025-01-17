using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    public static IngameUI instance { private set; get; }
    private readonly List<RaycastResult> resultCache;

    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup interactableCanvasGroup;
    [SerializeField] private Text interactableText;
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    [SerializeField] private TutorialScreen tutorialScreen;
    [SerializeField] private RulesScreen rulesScreen;
    [SerializeField] private HelpScreen helpScreen;
    [SerializeField] private AccountSettings accountScreen;
    [SerializeField] private AdminToolboxUI adminToolboxUI;

    [SerializeField] private ChatDisplay chatUIMaximized;
    [SerializeField] private ChatDisplay chatUIMinimized;
    [SerializeField] private ChatDisplay chatGame;

    [SerializeField] private Image centreDot;

    [SerializeField] public Button standUpButton;

    [SerializeField] private Color interactableNormalColour;
    [SerializeField] private Color interactableSelectedColour;

    public readonly EventVariable<IngameUI, IngameUIScreenType> selectedScreenType;

    private IngameUI()
    {
        resultCache = new List<RaycastResult>();
        selectedScreenType = new EventVariable<IngameUI, IngameUIScreenType>(this, IngameUIScreenType.None);
    }

    private void Awake()
    {
        instance = this;
        interactableCanvasGroup.alpha = 0;
        centreDot.color = interactableNormalColour;
        selectedScreenType.onValueChangeImmediate += OnValueChanged_SelectedScreenType;

        if (PlayerSettingsManager.instance.authLevel.value >= PlayerAuthLevel.Admin)
            adminToolboxUI.gameObject.SetActive(true);
        else
            adminToolboxUI.gameObject.SetActive(false);
    }

    private void OnValueChanged_SelectedScreenType(IngameUIScreenType oldValue, IngameUIScreenType newValue)
    {
        switch (oldValue)
        {
            case IngameUIScreenType.Tutorial:
                tutorialScreen.Hide();
                break;
            case IngameUIScreenType.Help:
                helpScreen.Hide();
                break;
            case IngameUIScreenType.Rules:
                rulesScreen.Hide();
                break;
            case IngameUIScreenType.AccountSettings:
                accountScreen.Hide();
                break;
        }

        switch (newValue)
        {
            case IngameUIScreenType.Tutorial:
                tutorialScreen.Show();
                break;
            case IngameUIScreenType.Help:
                helpScreen.Show();
                break;
            case IngameUIScreenType.Rules:
                rulesScreen.Show();
                break;
            case IngameUIScreenType.AccountSettings:
                accountScreen.Show();
                break;
        }

        if (newValue != IngameUIScreenType.None)
        {
            PlayerSettingsManager.instance.uiFocused.value = true;
        }
        else
        {
            PlayerSettingsManager.instance.uiFocused.value = false;
        }
    }

    private void Update()
    {
        if (PlayerController.instance == null) return;
        CheckIfShouldMovePlayer();
    }

    private void CheckIfShouldMovePlayer()
    {
        if (PlayerSettingsManager.instance.mouseMode.value == MouseMode.Game || PlayerSettingsManager.instance.uiFocused.value)
            return;
        
        if (!Input.GetMouseButtonDown(0))
            return;

        PointerEventData pointerEventData = new PointerEventData(null);
        pointerEventData.position = Input.mousePosition;
        resultCache.Clear();
        graphicRaycaster.Raycast(pointerEventData, resultCache);

        if (UIUtils.IsPointerOverUIElement(resultCache, "UI"))
            return;

        PlayerController.instance.TryNavigateToPosition();
    }

    public void ShowInteractableText(IInteractable interactable)
    {
        interactableText.text = interactable.interactText;
        interactableCanvasGroup.alpha = 1;
        centreDot.color = interactableSelectedColour;
    }

    public void HideInteractableText()
    {
        interactableCanvasGroup.alpha = 0;
        centreDot.color = interactableNormalColour;
    }

    public void ShowTutorialScreen()
    {
        tutorialScreen.Show();
    }

    private void OnDestroy()
    {
        selectedScreenType.onValueChangeImmediate -= OnValueChanged_SelectedScreenType;
    }

    public void SaveChatScroll()
    {
        if (PlayerSettingsManager.instance.mouseMode.value == MouseMode.Game)
        {
            chatGame.UpdateScrollbarValue();
        }
        else if (PlayerSettingsManager.instance.chatSizeMode.value == ChatSizeMode.Minimized) 
        {
            chatUIMinimized.UpdateScrollbarValue();
        }
        else
        {
            chatUIMaximized.UpdateScrollbarValue();
        }

        chatGame.ApplyScrollbarValue();
        chatUIMaximized.ApplyScrollbarValue();
        chatUIMinimized.ApplyScrollbarValue();
    }
}