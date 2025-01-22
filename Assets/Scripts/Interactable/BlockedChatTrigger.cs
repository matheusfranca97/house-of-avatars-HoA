
using UnityEngine;

public class BlockedChatTrigger : MonoBehaviour
{
    [SerializeField] private float triggerCooldown;

    private float nextPossibleTriggerTime;

    private void OnTriggerEnter(Collider other)
    {
        if (nextPossibleTriggerTime > Time.time)
            return;

        if (other.GetComponent<PlayerController>() == null)
            return;

        ChatLog.chatLogMessages.Add(new ChatLogMessage(-1, ChatMessageType.Whisper, PlayerAuthLevel.Server, "Guests cannot leave the Sanctuary area. Make an account to venture further."));
        nextPossibleTriggerTime = triggerCooldown + Time.time;
    }
}