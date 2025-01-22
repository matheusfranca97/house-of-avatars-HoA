using System.Collections;
using System.Linq;
using UnityEngine;

public class Interactable_CryptNPC : Interactable
{
    private const string CRYPT_NPC_NAME = "Lurch, the Groundskeeper";
    private const string CRYPT_NPC_CHAT_FORMAT = "Lurch, the Groundskeeper: {0}";

    public static Interactable_CryptNPC instance { private set; get; }
    [SerializeField] private AIChatResponse[] chatResponses;
    [field: SerializeField] public Transform chatLocation { private set; get; }

    protected override void Interact()
    {
        ChatInput.instance.PrepareChat_WhisperCryptNPC();
    }

    private void Awake()
    {
        instance = this;
    }

    public void ReceiveWhisper(string message)
    {

        Debug.Log(chatResponses.Length);
        AIChatResponse matchingResponse;
        if (!ChatResponseChecker.TryGetMatchingChatResponse(chatResponses, message, out matchingResponse))
            return;

        StartCoroutine(Routine_Respond(matchingResponse.response));
    }

    private IEnumerator Routine_Respond(string response)
    {
        yield return new WaitForSeconds(1);
        string chatMessage = string.Format(CRYPT_NPC_CHAT_FORMAT, response);
        ChatLog.chatLogMessages.Add(new ChatLogMessage(-1, ChatMessageType.Whisper, PlayerAuthLevel.Server, chatMessage).SetNPC());
        WorldSpaceChatController.instance.CreateWorldSpaceChat(chatLocation, CRYPT_NPC_NAME, chatMessage);
    }
}