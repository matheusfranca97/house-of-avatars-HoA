using UnityEngine;
using UnityEngine.UI;

public class WhisperInputField : MonoBehaviour
{
    [SerializeField] private Button whisperButtonOverlay;
    [SerializeField] private InputField whisperTargetInputField;

    private void Start()
    {
        if (PlayerSettingsManager.instance.authLevel.value < PlayerAuthLevel.User)
        {
            gameObject.SetActive(false);
        }

        whisperButtonOverlay.onClick.AddListener(OnPress_WhisperButtonOverlay);
        whisperTargetInputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnPress_WhisperButtonOverlay()
    {
        whisperTargetInputField.ActivateInputField();
        ChatInput.instance.chatMode.value = ChatMessageType.Whisper;
        PlayerSettingsManager.instance.uiFocused.value = true;
    }

    private void OnEndEdit(string newValue)
    {
        ChatInput.instance.chatMode.value = ChatMessageType.Whisper;
        PlayerSettingsManager.instance.uiFocused.value = false;
    }

    private void OnDestroy()
    {
        whisperButtonOverlay.onClick.RemoveListener(OnPress_WhisperButtonOverlay);
        whisperTargetInputField.onEndEdit.RemoveListener(OnEndEdit);
    }
}