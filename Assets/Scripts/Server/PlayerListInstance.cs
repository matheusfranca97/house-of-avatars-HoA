using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerListInstance : INetworkSerializable
{
    public ulong playerRef;
    public FixedString512Bytes displayName;

    public PlayerListInstance(ulong playerRef, string displayName)
    {
        this.playerRef = playerRef;
        this.displayName = new(displayName);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerRef);
        serializer.SerializeValue(ref displayName);
    }
}