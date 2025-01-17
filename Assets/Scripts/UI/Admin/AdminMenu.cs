using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class AdminMenu : MonoBehaviour
{
    public Button closeButton;
    public Button sendButton;

    public virtual void OnEnable()
    {
        sendButton.onClick.AddListener(Send);
    }

    public virtual void OnDisable()
    {
        sendButton.onClick.RemoveListener(Send);
    }

    protected bool IsUserValid(string user)
    {
        ulong player = PlayerList.instance.GetPlayerIDFromName(user);
        return player > 0;
    }

    protected void SendUserDoesntExist(string operation)
    {
        ChatLog.chatLogMessages.Add(new ChatLogMessage(-1, ChatMessageType.Whisper, PlayerAuthLevel.Server, $"User doesn't exist to {operation}"));
    }

    public abstract void Send();
}