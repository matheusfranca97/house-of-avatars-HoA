using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatInput : MonoBehaviour
{
    public static ChatInput instance { private set; get; }

    public EventVariable<ChatInput, ChatMessageType> chatMode;

    private const string PULPIT_NPC_NAME = "Seeker Lilith";
    private const string CRYPT_NPC_NAME = "Lurch";
    private const string SHOUT_CHAT_FORMAT = "{0}: {1}";
    private const string WHISPER_SEND_FORMAT = "To {0}: {1}";

    [SerializeField] private InputField whisperToInputField;
    [SerializeField] private InputField textInputField;
    [SerializeField] private Button sendMessageButton;

    public bool isFocussed => EventSystem.current.currentSelectedGameObject == textInputField.gameObject;

    private void Awake()
    {
        chatMode = new EventVariable<ChatInput, ChatMessageType>(this, ChatMessageType.Local);
        instance = this;
        sendMessageButton.interactable = false;
        sendMessageButton.onClick.AddListener(OnPress_SendMessageButton);
        textInputField.onValueChanged.AddListener(OnValueChanged_TextInputField);
        whisperToInputField.onValueChanged.AddListener(OnValueChanged_TextInputField);
        textInputField.onEndEdit.AddListener(OnEndEdit_TextInputField);
        whisperToInputField.onEndEdit.AddListener(OnEndEdit_TextInputField);
    }

    private void Start()
    {
        PlayerSettingsManager.instance.mouseMode.onValueChange += OnValueChanged_MouseMode;
    }

    private void OnValueChanged_MouseMode(MouseMode oldValue, MouseMode newValue)
    {
        if (newValue == MouseMode.Game)
        {
            textInputField.DeactivateInputField();
            whisperToInputField.DeactivateInputField();
        }
    }

    public void AddChatMessage(string message, ChatMessageType mode)
    {
        if ((mode == ChatMessageType.Shout || mode == ChatMessageType.Orator) && PlayerSettingsManager.instance.authLevel.value < PlayerAuthLevel.Admin)
        {
            string failedText = "You must be an orator or admin to shout";
            ChatLog.chatLogMessages.Add(new ChatLogMessage(
                -1,
                ChatMessageType.Whisper,
                PlayerSettingsManager.instance.authLevel.value,
                PlayerSettingsManager.instance.playerAvatarDataIndex.value,
                failedText));
            return;
        }

        string chatText = string.Format(SHOUT_CHAT_FORMAT, $"{PlayerSettingsManager.instance.gameUser.value.Username}", message);
        ChatLogMessage chatMessage = new(
            0,
            mode,
            PlayerSettingsManager.instance.authLevel.value,
            PlayerSettingsManager.instance.playerAvatarDataIndex.value,
            chatText);
        PlayerController.instance.networkController.ServerSendChatMessageRPC(NetworkManager.Singleton.LocalClientId, chatMessage);
    }

    public void AddChatMessage_SendWhisper(string recipient, string message)
    {
        ulong recipientPlayer = PlayerList.instance.GetPlayerIDFromName(recipient);
        if (recipient.Equals(PULPIT_NPC_NAME))
            Interactable_PulpitNPC.instance.ReceiveWhisper(message);
        else if (recipient.Equals(CRYPT_NPC_NAME))
            Interactable_CryptNPC.instance.ReceiveWhisper(message);
        else if (recipientPlayer == 0)
        {
            string failedText = "Recipient does not exist. Couldn't send message.";
            ChatLog.chatLogMessages.Add(new ChatLogMessage(
                -1,
                ChatMessageType.Whisper,
                PlayerSettingsManager.instance.authLevel.value,
                PlayerSettingsManager.instance.playerAvatarDataIndex.value,
                failedText)); //This is local
            return;
        }
        else
        {

            //string chatText = string.Format(WHISPER_SEND_FORMAT, recipient, message); This is now added by server
            ChatLogMessage chatMessage = new ChatLogMessage(
                0,
                ChatMessageType.Whisper,
                PlayerSettingsManager.instance.authLevel.value,
                PlayerSettingsManager.instance.playerAvatarDataIndex.value,
                message);
            PlayerController.instance.networkController.ServerSendWhisperMessageRPC(NetworkManager.Singleton.LocalClientId, recipientPlayer, chatMessage);
        }
    }

    public void PrepareChat_WhisperCryptNPC()
    {
        PlayerSettingsManager.instance.mouseMode.value = MouseMode.UI;
        whisperToInputField.SetTextWithoutNotify(CRYPT_NPC_NAME);
        textInputField.ActivateInputField();
        chatMode.value = ChatMessageType.Whisper;
    }

    public void PrepareChat_WhisperPulpitNPC()
    {
        PlayerSettingsManager.instance.mouseMode.value = MouseMode.UI;
        whisperToInputField.SetTextWithoutNotify(PULPIT_NPC_NAME);
        textInputField.ActivateInputField();
        chatMode.value = ChatMessageType.Whisper;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == textInputField.gameObject && Input.GetKeyDown(KeyCode.Return) && textInputField.text.Length > 0)
        {
            SendMessage();
            textInputField.ActivateInputField();
        }
        else if (EventSystem.current.currentSelectedGameObject != textInputField.gameObject && Input.GetKeyDown(KeyCode.Return))
        {
            textInputField.ActivateInputField();
        }
    }

    private void OnValueChanged_TextInputField(string text)
    {
        sendMessageButton.interactable = text.Length > 0;
        PlayerSettingsManager.instance.uiFocused.value = true;
    }

    private void OnEndEdit_TextInputField(string _text) //_ to show it it's a placeholder
    {
        if (IngameUI.instance.selectedScreenType.value != IngameUIScreenType.None)
        {
            return;
        }
        PlayerSettingsManager.instance.uiFocused.value = false;
    }

    private void OnPress_SendMessageButton()
    {
        SendMessage();
    }

    private void SendMessage()
    {
        if (PlayerSettingsManager.instance.isMuted.value && chatMode.value != ChatMessageType.Pray)
        {
            ChatLogMessage failMessage = new(
                -1,
                ChatMessageType.Whisper,
                PlayerSettingsManager.instance.authLevel.value,
                PlayerSettingsManager.instance.playerAvatarDataIndex.value,
                "You have been muted and cannot message");
            ChatLog.chatLogMessages.Add(failMessage);
            textInputField.SetTextWithoutNotify(string.Empty);
            return;
        }

        if (chatMode.value == ChatMessageType.Pray)
            ChatLog.chatLogMessages.Add(new ChatLogMessage(0, ChatMessageType.Pray, PlayerAuthLevel.User, PlayerSettingsManager.instance.playerAvatarDataIndex.value, textInputField.text));
        else if (chatMode.value == ChatMessageType.Local)
            AddChatMessage(textInputField.text, ChatMessageType.Local);
        else if (chatMode.value == ChatMessageType.Shout)
            AddChatMessage(textInputField.text, ChatMessageType.Shout);
        else if (chatMode.value == ChatMessageType.Orator)
            AddChatMessage(textInputField.text, ChatMessageType.Orator);
        else
            AddChatMessage_SendWhisper(whisperToInputField.text, textInputField.text);

        textInputField.SetTextWithoutNotify(string.Empty);
    }

    private void OnDestroy()
    {
        sendMessageButton.onClick.RemoveListener(OnPress_SendMessageButton);
        textInputField.onValueChanged.RemoveListener(OnValueChanged_TextInputField);
        PlayerSettingsManager.instance.mouseMode.onValueChange -= OnValueChanged_MouseMode;
        textInputField.onEndEdit.RemoveListener(OnEndEdit_TextInputField);
    }
}