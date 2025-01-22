
public static class ChatLog
{
    private const int MAX_MESSAGES = 100;
    public static readonly EventList<ChatLogMessage> chatLogMessages;

    static ChatLog()
    {
        chatLogMessages = new EventList<ChatLogMessage>();
        chatLogMessages.onAdd += OnAdd_ChatLogMessage;
        ResetChat();
    }

    private static void OnAdd_ChatLogMessage(ChatLogMessage chatLogMessage)
    {
        if (chatLogMessages.Count <= MAX_MESSAGES)
            return;

        chatLogMessages.RemoveAt(MAX_MESSAGES);
    }

    public static void ResetChat()
    {
        chatLogMessages.Clear();
        chatLogMessages.Add(new ChatLogMessage(-1, ChatMessageType.Whisper, PlayerAuthLevel.Server, "Welcome to House of Avatars! These walls have ears. Invisible guests may be lurking and able to see your conversations"));
    }
}