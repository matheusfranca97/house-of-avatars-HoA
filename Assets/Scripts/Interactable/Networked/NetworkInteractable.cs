using Unity.Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class NetworkInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] public string interactText { get; set; }
    public EventVariable<IInteractable, bool> CanInteract { get; }

    protected NetworkInteractable()
    {
        CanInteract = new EventVariable<IInteractable, bool>(this, true);
    }

    private void Start()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            ServerGetCanInteractRPC();
        }
    }

    public void TriggerInteract()
    {
        LocalInteract(NetworkManager.Singleton.LocalClientId);
        ServerInteractRPC(NetworkManager.Singleton.LocalClientId);
    }

    protected abstract void Interact(bool isTriggerPlayer, bool isGuest);

    [Rpc(SendTo.Server)]
    public virtual void ServerInteractRPC(ulong sendingPlayer)
    {
        //Send to all other clients
        ClientRecieveInteractRPC(sendingPlayer);
    }

    [Rpc(SendTo.NotServer)]
    public void ClientRecieveInteractRPC(ulong sendingPlayer)
    {
        if (NetworkManager.Singleton.LocalClientId == sendingPlayer)
        {
            return;
        }
        LocalInteract(sendingPlayer);
    }

    private void LocalInteract(ulong sendingPlayer)
    {
        bool isGuest = PlayerSettingsManager.instance.authLevel.value is PlayerAuthLevel.Guest;
        if (NetworkManager.Singleton.LocalClientId == sendingPlayer)
            Interact(true, isGuest);
        else
            Interact(false, isGuest);
    }

    [Rpc(SendTo.Server)]
    public void ServerSetSittableRPC(ulong playerRef)
    {
        DedicatedServerManager.instance.SetPlayerSittingInteractable(playerRef, this);
    }

    [Rpc(SendTo.Server)]
    public void ServerUnsetSittableRPC(ulong playerRef)
    {
        DedicatedServerManager.instance.UnsetPlayerSittingInteractable(playerRef);
    }

    public void SetCanInteract(bool canInteract)
    {
        if (NetworkManager.Singleton.IsServer || PlayerSettingsManager.instance.authLevel.value != PlayerAuthLevel.Guest) 
        {
            ServerSetCanInteractRPC(canInteract);
        }
    }

    [Rpc(SendTo.Server)]
    private void ServerSetCanInteractRPC(bool canInteract)
    {
        CanInteract.value = canInteract;
        ClientSetCanInteractRPC(canInteract);
    }

    [Rpc(SendTo.NotServer)]
    private void ClientSetCanInteractRPC(bool canInteract)
    {
        CanInteract.value = canInteract;
        Debug.Log($"CanInteract: {canInteract.ToString()}");
    }

    [Rpc(SendTo.Server)]
    private void ServerGetCanInteractRPC()
    {
        ClientSetCanInteractRPC(CanInteract.value);
    }

    private void OnDestroy()
    {
        CanInteract.value = false;
    }
}
