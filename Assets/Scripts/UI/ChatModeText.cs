
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChatModeText: MonoBehaviour
{
    private Text text;
    [SerializeField] private Color localModeColor;
    [SerializeField] private Color shoutModeColor;
    [SerializeField] private Color whisperModeColor;
    [SerializeField] private Color oratorModeColor;
    [SerializeField] private Color prayModeColor;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
        ChatInput.instance.chatMode.onValueChangeImmediate += OnValueChanged_ChatMode;
    }

    private void OnValueChanged_ChatMode(ChatMessageType oldValue, ChatMessageType newValue)
    {
        if (newValue == ChatMessageType.Local)
            text.color = localModeColor;
        else if (newValue == ChatMessageType.Whisper)
            text.color = whisperModeColor;
        else if (newValue == ChatMessageType.Orator)
            text.color = oratorModeColor;
        else if (newValue == ChatMessageType.Pray)
            text.color = prayModeColor;
        else
            text.color = shoutModeColor;
    }

    private void OnDestroy()
    {
        ChatInput.instance.chatMode.onValueChange -= OnValueChanged_ChatMode;
    }
}