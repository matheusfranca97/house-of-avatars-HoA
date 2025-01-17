
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MaximizeChatModeButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        IngameUI.instance.SaveChatScroll();
        PlayerSettingsManager.instance.chatSizeMode.value = ChatSizeMode.Maximized;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}