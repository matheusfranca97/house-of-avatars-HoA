using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.XR;

public class PlayerAvatar : NetworkBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public NetworkAnimator networkAnimator;
    [SerializeField] public EventBridge_PlayerCharacter eventBridge;
    [SerializeField] public Transform characterRotationTransform;
    [SerializeField] public ClientNetworkTransform clientNetworkTransform;
    public SkinnedMeshRenderer[] meshRenderers { set; get; }

    public NetworkPlayer player;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public bool CheckVisibility(ulong playerRef)
    {
        if (!IsSpawned)
        {
            return false;
        }

        if (player == null)
        {
            return false;
        }

        if (playerRef == player.playerRef)
        {
            return true; //Will be hidden locally by that player, needed for NetworkPlayerController initialization
        }

        NetworkPlayer netPlayer = DedicatedServerManager.instance.GetPlayerFromID(playerRef);
        if (netPlayer.authLevel is PlayerAuthLevel.Admin)
        {
            return true;
        }
        else if (player.authLevel is PlayerAuthLevel.Guest)
        {
            return false;
        }
        else if (player.authLevel is PlayerAuthLevel.Admin)
        {
            return !player.isHidden;
        }
        else
        {
            return true;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkObject.CheckObjectVisibility += CheckVisibility;
            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
        }
        base.OnNetworkSpawn();
    }

    private void OnNetworkTick()
    {
        foreach (ulong playerRef in NetworkManager.ConnectedClientsIds)
        {
            bool result = CheckVisibility(playerRef);
            bool isVisible = NetworkObject.IsNetworkVisibleTo(playerRef);
            if (result && !isVisible)
            {
                NetworkObject.NetworkShow(playerRef);
            }
            else if (!result && isVisible)
            {
                NetworkObject.NetworkHide(playerRef);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkObject.CheckObjectVisibility -= CheckVisibility;
            NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        }
        base.OnNetworkDespawn();
    }
}
