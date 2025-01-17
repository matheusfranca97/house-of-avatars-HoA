using System.Collections;
using UnityEngine;

public class Interactable_PulpitNPC : Interactable
{
    private const string PULPIT_NPC_NAME = "Seeker Lilith";
    private const string PULPIT_NPC_CHAT_FORMAT = "Seeker Lilith: {0}";

    public static Interactable_PulpitNPC instance { private set; get; }
    [SerializeField] private AIChatResponse[] chatResponses;
    [field: SerializeField] public Transform chatLocation { private set; get; }

    protected override void Interact()
    {
        ChatInput.instance.PrepareChat_WhisperPulpitNPC();
    }

    private void Awake()
    {
        instance = this;
    }

    public void ReceiveWhisper(string message)
    {
        AIChatResponse matchingResponse;
        if (!ChatResponseChecker.TryGetMatchingChatResponse(chatResponses, message, out matchingResponse))
            return;

        StartCoroutine(Routine_Respond(matchingResponse.response));
    }

    private IEnumerator Routine_Respond(string response)
    {
        yield return new WaitForSeconds(1);
        string chatMessage = string.Format(PULPIT_NPC_CHAT_FORMAT, response);
        ChatLog.chatLogMessages.Add(new ChatLogMessage(-1, ChatMessageType.Whisper, PlayerAuthLevel.Server, chatMessage).SetNPC());
        WorldSpaceChatController.instance.CreateWorldSpaceChat(chatLocation, PULPIT_NPC_NAME, chatMessage);
    }
}