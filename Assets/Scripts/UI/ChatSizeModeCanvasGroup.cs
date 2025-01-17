
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ChatSizeModeCanvasGroup : MonoBehaviour
{
    [SerializeField] private ChatSizeMode chatSizeMode;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        PlayerSettingsManager.instance.chatSizeMode.onValueChangeImmediate += OnValueChanged_ChatSizeMode;
    }

    private void OnValueChanged_ChatSizeMode(ChatSizeMode oldValue, ChatSizeMode newValue)
    {
        if (chatSizeMode == newValue)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDestroy()
    {
        PlayerSettingsManager.instance.chatSizeMode.onValueChange -= OnValueChanged_ChatSizeMode;
    }
}