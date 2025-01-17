using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

public class NetworkPlayer
{ 
    //Initial Contact
    public GameUser gameUser;
    public string supabaseUID;
    public string authUID;
    public ulong playerRef;
    public string displayName;
    public PlayerAuthLevel authLevel;
    public short avatarIndex;

    //Spawn contact
    public NetworkPlayerController playerObject;
    public PlayerAvatar avatar;

    //Dynamics
    public bool isMuted;
    public bool isHidden = false;

    public NetworkInteractable sittingInteractable = null;

    public static NetworkPlayer PopulateInitial(GameUser gameUser, string supabaseUID, ulong playerRef, string displayName, PlayerAuthLevel authLevel, short avatarIndex, string authUID)
    {
        NetworkPlayer player = new();
        player.gameUser = gameUser;
        player.supabaseUID = supabaseUID;
        player.authUID = authUID;
        player.playerRef = playerRef;
        player.displayName = displayName;
        player.authLevel = authLevel;
        player.avatarIndex = avatarIndex;
        return player;
    }
}