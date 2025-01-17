
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform chatMessagesContainer;
    [SerializeField] private RectTransform prayMessagesContainer;
    [SerializeField] private Scrollbar scrollbar;    

    [SerializeField] private ChatMessage chatMessagePrefab;
    

    private ChatMessage chatMessage;
    private ChatMessage prayChatMessage;
    private bool scrollDown = false;

    // private readonly List<ChatMessage> chatMessageInstances;

    //private ChatDisplay()
    //{
    //    chatMessageInstances = new List<ChatMessage>();
    //}

    private void Awake()
    {
        ChatLog.chatLogMessages.onAdd += OnAdd_ChatLogMessage;
        ChatLog.chatLogMessages.onRemove += OnRemove_ChatLogMessage;
        foreach (ChatLogMessage chatLogMessage in ChatLog.chatLogMessages)
            OnAdd_ChatLogMessage(chatLogMessage);
    }

    private void Start()
    {
        ChatInput.instance.chatMode.onValueChange += ChatMode_onValueChange;
    }

    private void ChatMode_onValueChange(ChatMessageType oldValue, ChatMessageType newValue)
    {
        if (newValue is ChatMessageType.Pray)
        {
            chatMessagesContainer.gameObject.SetActive(false);
            prayMessagesContainer.gameObject.SetActive(true);
        }
        else if (oldValue is ChatMessageType.Pray)
        {
            chatMessagesContainer.gameObject.SetActive(true);
            prayMessagesContainer.gameObject.SetActive(false);
        }
    }

    private void OnAdd_ChatLogMessage(ChatLogMessage item)
    {
        if (scrollbar.value == 0)
        {
            scrollDown = true;
        }

        if (item.messageType is ChatMessageType.Pray || item.messageType is ChatMessageType.Orator)
        {
            ChatMessage prayInstance = GetPrayChatMessage();
            prayInstance.AddChatLogMessage(item);
        }

        if (item.messageType != ChatMessageType.Pray)
        {
            ChatMessage instance = GetChatMessage();
            instance.AddChatLogMessage(item);
        }
    }

    private void FixedUpdate()
    {
        if (scrollDown)
        {
            scrollbar.value = 0;
            scrollDown = false;
        }
    }

    private void OnRemove_ChatLogMessage(ChatLogMessage item)
    {
        //for(int i = 0; i < chatMessageInstances.Count; i++)
        //{
        //    ChatMessage instance = chatMessageInstances[i];

        //    if (instance.TryRemoveChatLogMessage(item))
        //        break;
        //}

        if (chatMessage != null)
        {
            chatMessage.TryRemoveChatLogMessage(item);
        }
    }

    private void OnDestroy()
    {
        ChatLog.chatLogMessages.onAdd -= OnAdd_ChatLogMessage;
        ChatLog.chatLogMessages.onRemove -= OnRemove_ChatLogMessage;
        ChatInput.instance.chatMode.onValueChange -= ChatMode_onValueChange;
    }

    private ChatMessage GetPrayChatMessage()
    {
        if (prayChatMessage == null) 
        {
            prayChatMessage = Instantiate(chatMessagePrefab, prayMessagesContainer);
        }
        return prayChatMessage;
    }

    private ChatMessage GetChatMessage()
    {
        if (chatMessage != null)
            return chatMessage;

        chatMessage = Instantiate(chatMessagePrefab, chatMessagesContainer);
        return chatMessage;

        //if (chatMessageInstances.Count > 0)
        //{
        //    ChatMessage lastMessage = chatMessageInstances[chatMessageInstances.Count - 1];
        //    if (lastMessage.chatMessageType == type)
        //        return lastMessage;
        //}

        //switch (type)
        //{
        //    case ChatMessageType.Local:
        //        {
        //            ChatMessage instance = Instantiate(localChatMessagePrefab, chatMessagesContainer);
        //            chatMessageInstances.Add(instance);
        //            return instance;
        //        }
        //    case ChatMessageType.Shout:
        //        {
        //            ChatMessage instance = GameObject.Instantiate(shoutChatMessagePrefab, chatMessagesContainer);
        //            chatMessageInstances.Add(instance);
        //            return instance;
        //        }
        //    case ChatMessageType.Whisper:
        //        {
        //            ChatMessage instance = GameObject.Instantiate(whisperChatMessagePrefab, chatMessagesContainer);
        //            chatMessageInstances.Add(instance);
        //            return instance;
        //        }
        //}

        //throw new System.Exception(string.Format("GetChatMessage is missing implementation for {0}", type));
    }

    public void UpdateScrollbarValue()
    {
        PlayerSettingsManager.instance.chatScrollValue.value = scrollbar.value;
    }

    public void ApplyScrollbarValue()
    {
        scrollbar.value = PlayerSettingsManager.instance.chatScrollValue.value;
    }
}