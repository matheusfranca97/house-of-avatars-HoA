using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

public class StringContainer : INetworkSerializable
{
    public string text;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(text);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out text);
        }
    }

    public StringContainer(string text)
    {
        this.text = text;
    }
    public StringContainer() { }
}
