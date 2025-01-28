using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PrayMicrophoneToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        //toggle.onValueChanged.AddListener(OnToggled);
        ChatInput.instance.chatMode.onValueChange += OnChatModeChanged;
        if (ChatInput.instance.chatMode.value != ChatMessageType.Pray)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnChatModeChanged(ChatMessageType oldValue, ChatMessageType newValue)
    {
        // if (newValue is ChatMessageType.Pray)
        // {
        //     toggle.SetIsOnWithoutNotify(true);
        //     gameObject.SetActive(true);
        // }
        // else if (oldValue is ChatMessageType.Pray)
        // {
        //     gameObject.SetActive(false);
        // }
    }

    private void OnDestroy()
    {
        //toggle.onValueChanged.RemoveListener(OnToggled);
        ChatInput.instance.chatMode.onValueChange -= OnChatModeChanged;
    }
}
