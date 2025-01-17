using UnityEngine;
using UnityEngine.UI;

public class ChatText : DataDrivenUI<ChatLogMessage>
{
    [SerializeField] private Text textField;
    [SerializeField] private Image iconField;
    [SerializeField] private Color oratorBackgroundColour;
    [SerializeField] private Color adminBackgroundColour;
    [SerializeField] public Image background;

    protected override void OnValueChanged_Data(ChatLogMessage oldValue, ChatLogMessage newValue)
    {
        if(newValue != null)
        {
            //Set message text
            textField.text = newValue.message.Value;
            background.gameObject.SetActive(false);

            //Set text colour
            if (newValue.senderAuthLevel is PlayerAuthLevel.Admin)
            {
                if (newValue.messageType is ChatMessageType.Local || newValue.messageType is ChatMessageType.Whisper)
                {
                    textField.color = Color.white;
                }
                else if (newValue.messageType is ChatMessageType.Shout)
                {
                    textField.color = Color.white;
                    background.color = adminBackgroundColour;
                    background.gameObject.SetActive(true);
                }
                else if (newValue.messageType is ChatMessageType.Orator)
                {
                    textField.color = Color.black;
                    background.color = oratorBackgroundColour;
                    background.gameObject.SetActive(true);
                }
            }
            else if (newValue.isNPC)
                textField.color = Color.magenta;

            //Set icon
            if (iconField == null) return;

            if (newValue.senderAuthLevel is PlayerAuthLevel.Server) 
                iconField.sprite = AvatarManager.instance.serverUserIcon;
            else
                iconField.sprite = AvatarManager.instance.avatarList.avatarDataList[newValue.senderAvatarIndex].avatarChatIcon;
        }
    }
}