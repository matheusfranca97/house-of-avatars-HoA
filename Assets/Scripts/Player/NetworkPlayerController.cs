using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using static Unity.Collections.Unicode;

public class NetworkPlayerController : NetworkBehaviour
{
    private PlayerAvatar remoteAvatar;
    private PlayerAvatar localAvatar;

    #region OnSpawn
    [Rpc(SendTo.Owner)]
    public void ClientSpawnPlayerControllerRPC(NetworkObjectReference avatarObjectRef, NetworkObjectReference ottDisplayRef)
    {
        SpawnPlayerController(avatarObjectRef, ottDisplayRef);
    }

    private async void SpawnPlayerController(NetworkObjectReference avatarObjectRef, NetworkObjectReference ottDisplayRef)
    {
        NetworkObject avatarObject;
        while (!avatarObjectRef.TryGet(out avatarObject) || PlayerController.instance == null)
        {
            await Task.Yield();
        }

        //Setup player controller
        PlayerController.instance.networkController = this;
        remoteAvatar = avatarObject.GetComponent<PlayerAvatar>();

        //Recreate avatar locally for latency
        localAvatar = Instantiate(remoteAvatar.gameObject, PlayerController.instance.transform).GetComponent<PlayerAvatar>();
        PlayerController.instance.SetPlayerAvatar(localAvatar);

        PlayerController.instance.WarpPosition(transform);

        foreach (Transform child in remoteAvatar.transform)
        {
            remoteAvatar.gameObject.SetActive(false); //Hide remote avatar locally
        }

        NetworkObject ottObject;
        while (!ottDisplayRef.TryGet(out ottObject))
        {
            await Task.Yield();
        }
        ottObject.GetComponent<NetworkOTTDisplay>().Setup();

        SceneController.completeLoad = true;
    }
    #endregion
    #region Player Movement / Animation
    //[Rpc(SendTo.Server)]
    //public void ServerAvatarSetTriggerRPC(ulong playerRef, string newTrigger)
    //{
    //    DedicatedServerManager.instance.SetAvatarAnimationTrigger(playerRef, newTrigger);
    //}

    //[Rpc(SendTo.Server)]
    //public void ServerAvatarResetTriggerRPC(ulong playerRef, string oldTrigger)
    //{
    //    DedicatedServerManager.instance.ResetAvatarAnimationTrigger(playerRef, oldTrigger);
    //}

    [Rpc(SendTo.Server)]
    public void ServerAvatarSetResetTriggersRPC(ulong playerRef, string oldTrigger, string newTrigger)
    {
        DedicatedServerManager.instance.SetResetAvatarAnimationTriggers(playerRef, oldTrigger, newTrigger);
    }

    [Rpc(SendTo.Server)]
    public void ServerAvatarSetAnimatorSpeedsRPC(ulong playerRef, StringContainer[] keys, float[] values)
    {
        DedicatedServerManager.instance.SetAnimationSpeed(playerRef, keys, values);
    }

    public void UpdateAvatar()
    {
        remoteAvatar.transform.position = PlayerController.instance.transform.position;
        remoteAvatar.transform.rotation = PlayerController.instance.characterRotationTransform.rotation;
    }
    #endregion
    #region Chat Messages
    [Rpc(SendTo.Server)]
    public void ServerToggleOratorModeRPC(ulong playerRef, bool toggled)
    {
        DedicatedServerManager.instance.PlayerOratorModeToggled(playerRef, toggled);
    }

    [Rpc(SendTo.Server)]
    public void ServerSendChatMessageRPC(ulong sender, ChatLogMessage message)
    {
        DedicatedServerManager.instance.SendChatMessage(sender, message);
    }

    [Rpc(SendTo.Server)]
    public void ServerSendWhisperMessageRPC(ulong sender, ulong reciever, ChatLogMessage message, string recipientName = "")
    {
        Debug.Log("rpc sending message");
        DedicatedServerManager.instance.SendWhisperMessage(sender, reciever, message, recipientName);
    }

    [Rpc(SendTo.Owner)]
    public void ClientRecieveMessageRPC(ChatLogMessage message)
    {
        ChatLog.chatLogMessages.Add(message);
    }
    #endregion
    #region User Management
    [Rpc(SendTo.Server)]
    public void ServerDeleteUserRPC(ulong sender)
    {
        DedicatedServerManager.instance.DeleteUser(sender);
    }
    #endregion
    #region Admin Toolbox
    [Rpc(SendTo.Owner)]
    public void ClientGetMutedRPC()
    {
        PlayerSettingsManager.instance.isMuted.value = true;
    }

    [Rpc(SendTo.Server)]
    public void ServerMutePlayerRPC(ulong sender, ulong target)
    {
        DedicatedServerManager.instance.MutePlayer(sender, target);
    }

    [Rpc(SendTo.Server)]
    public void ServerKickPlayerRPC(ulong sender, ulong target)
    {
        DedicatedServerManager.instance.KickPlayer(sender, target);
    }

    [Rpc(SendTo.Server)]
    public void ServerBanPlayerRPC(ulong sender, ulong target)
    {
        DedicatedServerManager.instance.BanPlayer(sender, target);
    }

    [Rpc(SendTo.Server)]
    public void ServerToggleInvisibleRPC(ulong sender, bool visible)
    {
        DedicatedServerManager.instance.ToggleVisibility(sender, visible);
    }

    [Rpc(SendTo.Server)]
    public void ServerTeleportRPC(ulong sender, Vector3 position, Quaternion rotation, string locationName)
    {
        DedicatedServerManager.instance.Teleport(sender, position, rotation, locationName);
    }

    [Rpc(SendTo.Owner)]
    public void ClientRecieveTeleportRPC(Vector3 position, Quaternion rotation)
    {
        PlayerController.instance.WarpPosition(position, rotation);
    }
    #endregion
}