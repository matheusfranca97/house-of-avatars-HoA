
using UnityEngine;
using UnityEngine.UI;

public class OratorButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        if (PlayerSettingsManager.instance.authLevel.value < PlayerAuthLevel.Admin)
        {
            gameObject.SetActive(false);
        }

        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        ChatInput.instance.chatMode.value = ChatMessageType.Orator;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}