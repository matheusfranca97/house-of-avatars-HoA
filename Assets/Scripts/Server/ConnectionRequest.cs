using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ConnectionRequest
{
    public string username;
    public string supabaseUID;
    public string accessToken;
    public short avatarIndex;
    public string authUID;
    public bool isGuest;

    public ConnectionRequest(string username, string supabaseUID, string accessToken, short avatarIndex, string authUID, bool isGuest)
    {
        this.username = username;
        this.supabaseUID = supabaseUID;
        this.accessToken = accessToken;
        this.avatarIndex = avatarIndex;
        this.authUID = authUID;
        this.isGuest = isGuest;
    }

    public ConnectionRequest() { }

    public byte[] Serialize()
    {
        using (MemoryStream memoryStream = new())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(username);
                writer.Write(supabaseUID);
                writer.Write(accessToken);
                writer.Write(avatarIndex);
                writer.Write(authUID);
                writer.Write(isGuest);
            };
            return memoryStream.ToArray();
        }
    }

    public static ConnectionRequest Deserialize(byte[] data)
    {
        ConnectionRequest result = new();
        using (MemoryStream memoryStream = new(data))
        {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                result.username = reader.ReadString();
                result.supabaseUID = reader.ReadString();
                result.accessToken = reader.ReadString();
                result.avatarIndex = reader.ReadInt16();
                result.authUID = reader.ReadString();
                result.isGuest = reader.ReadBoolean();
            }
        }
        return result;
    }
}