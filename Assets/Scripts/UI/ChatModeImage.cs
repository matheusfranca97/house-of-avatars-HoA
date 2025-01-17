
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChatModeImage : MonoBehaviour
{
    private Image image;
    [SerializeField] private Color localModeColor;
    [SerializeField] private Color shoutModeColor;
    [SerializeField] private Color whisperModeColor;
    [SerializeField] private Color oratorModeColor;
    [SerializeField] private Color prayModeColor;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        ChatInput.instance.chatMode.onValueChangeImmediate += OnValueChanged_ChatMode;
    }

    private void OnValueChanged_ChatMode(ChatMessageType oldValue, ChatMessageType newValue)
    {
        if (newValue == ChatMessageType.Local)
            image.color = localModeColor;
        else if (newValue == ChatMessageType.Whisper)
            image.color = whisperModeColor;
        else if (newValue == ChatMessageType.Orator)
            image.color = oratorModeColor;
        else if (newValue == ChatMessageType.Pray)
            image.color = prayModeColor;
        else
            image.color = shoutModeColor;
    }

    private void OnDestroy()
    {
        ChatInput.instance.chatMode.onValueChange -= OnValueChanged_ChatMode;
    }
}