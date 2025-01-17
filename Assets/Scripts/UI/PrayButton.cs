using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PrayButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        if (ChatInput.instance.chatMode.value is ChatMessageType.Pray)
        {
            ChatInput.instance.chatMode.value = ChatMessageType.Local;
        }
        else
        {
            ChatInput.instance.chatMode.value = ChatMessageType.Pray;
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}