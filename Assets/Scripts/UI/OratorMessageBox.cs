using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class OratorMessageBox : MonoBehaviour
{
    public static OratorMessageBox instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text oratorMessage;

    private void Awake()
    {
        instance = this;
        ToggleBox(false);
    }

    public void ToggleBox(bool enabled)
    {
        canvasGroup.alpha = enabled ? 1 : 0;
    }

    public void SetOratorMessage(string message)
    {
        oratorMessage.text = message;
        LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
    }

    //private void Start()
    //{
    //    instance = this;
    //    ChatLog.chatLogMessages.onAdd += OnAdd_ChatLogMessage;
    //    ChatInput.instance.chatMode.onValueChange += ChatMode_onValueChange;
    //    canvasGroup.alpha = 0;
    //}

    //private void OnDestroy()
    //{
    //    ChatLog.chatLogMessages.onAdd -= OnAdd_ChatLogMessage;
    //    ChatInput.instance.chatMode.onValueChange -= ChatMode_onValueChange;
    //}

    //private void OnAdd_ChatLogMessage(ChatLogTextMessage item)
    //{
    //    if (item.message.messageType != (int)ChatMessageType.Orator)
    //    {
    //        return;
    //    }

    //    SetOratorMessage(item);
    //}

    //private void ChatMode_onValueChange(ChatMessageType oldValue, ChatMessageType newValue)
    //{
    //    PlayerController.instance.fusionNetworkController.Admin_ToggleOratorMode(newValue == ChatMessageType.Orator);
    //}


}