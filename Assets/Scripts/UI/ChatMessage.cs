
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private ChatText chatTextPrefab;
    [SerializeField] private RectTransform chatTextContainer;
    private readonly List<ChatText> chatTextInstances;

    private ChatMessage()
    {
        chatTextInstances = new List<ChatText>();
    }

    public void AddChatLogMessage(ChatLogMessage chatLogMessage)
    {
        ChatText instance = Instantiate(chatTextPrefab, chatTextContainer);
        instance.data = chatLogMessage;
        chatTextInstances.Add(instance);
    }

    public bool TryRemoveChatLogMessage(ChatLogMessage chatLogMessage)
    {
        ChatText instance;
        if (!chatTextInstances.TryFind(i => i.data.id == chatLogMessage.id, out instance))
            return false;

        Destroy(instance.gameObject);
        chatTextInstances.Remove(instance);
        return true;
    }
}