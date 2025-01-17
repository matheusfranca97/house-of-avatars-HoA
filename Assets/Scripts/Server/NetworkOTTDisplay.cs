using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;

public class NetworkOTTDisplay : NetworkBehaviour
{
    public NetworkVariable<bool> isActive = new(false);
    public NetworkVariable<FixedString512Bytes> currentMessage = new(new(""));

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
        {
            return;
        }

        isActive.OnValueChanged += OnActiveChanged;
        currentMessage.OnValueChanged += OnMessageChanged;
    }

    public void Setup()
    {
        ChatInput.instance.chatMode.onValueChange += OnChatModeChanged;
        OnActiveChanged(false, isActive.Value);
        OnMessageChanged("", currentMessage.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient)
        {
            return;
        }

        isActive.OnValueChanged -= OnActiveChanged;
        currentMessage.OnValueChanged -= OnMessageChanged;

        ChatInput.instance.chatMode.onValueChange -= OnChatModeChanged;
    }

    private void OnActiveChanged(bool previous, bool current)
    {
        if (OratorMessageBox.instance == null)
        {
            return;
        }

        OratorMessageBox.instance.ToggleBox(current);
    }

    private void OnMessageChanged(FixedString512Bytes previous, FixedString512Bytes current)
    {
        if (OratorMessageBox.instance == null)
        {
            return;
        }

        OratorMessageBox.instance.SetOratorMessage(current.ToString());
    }

    private void OnChatModeChanged(ChatMessageType previous, ChatMessageType current)
    {
        if ((previous is ChatMessageType.Orator && current is not ChatMessageType.Orator) ||
            current is ChatMessageType.Orator && previous is not ChatMessageType.Orator)
        {
            PlayerController.instance.networkController.ServerToggleOratorModeRPC(NetworkManager.Singleton.LocalClientId, current is ChatMessageType.Orator);
        }
    }
}