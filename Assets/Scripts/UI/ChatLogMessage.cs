using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;

public class ChatLogMessage : INetworkSerializable
{
    public int id;
    public ChatMessageType messageType;
    public FixedString512Bytes message;

    public short senderAvatarIndex;
    public PlayerAuthLevel senderAuthLevel;
    public bool isNPC;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref messageType);
        serializer.SerializeValue(ref message);

        serializer.SerializeValue(ref senderAvatarIndex);
        serializer.SerializeValue(ref senderAuthLevel);
        serializer.SerializeValue(ref isNPC);
    }

    public ChatLogMessage() { }

    public ChatLogMessage(int id, ChatMessageType type, PlayerAuthLevel senderAuthLevel, string message)
    {
        this.id = id;
        this.messageType = type;
        this.senderAuthLevel = senderAuthLevel;
        this.message = new(message);
    }

    public ChatLogMessage(int id, ChatMessageType type, PlayerAuthLevel senderAuthLevel, short senderAvatarIndex, string message)
    {
        this.id = id;
        this.messageType = type;
        this.senderAuthLevel = senderAuthLevel;
        this.senderAvatarIndex = senderAvatarIndex;
        this.message = new(message);
    }

    public void SetID(int id)
    {
        this.id = id; //Should be set by server
    }

    public ChatLogMessage SetIDInline(int id)
    {
        this.id = id;
        return this;
    }

    public ChatLogMessage SetNPC()
    {
        this.isNPC = true;
        return this;
    }

    public static ChatLogMessage FromServer(string message)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return null;
        }

        return new ChatLogMessage(-1, ChatMessageType.Whisper, PlayerAuthLevel.Server, message);
    }
}
