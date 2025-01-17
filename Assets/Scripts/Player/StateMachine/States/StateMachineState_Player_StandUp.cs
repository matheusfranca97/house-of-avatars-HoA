using System;
using Unity.Netcode;
using UnityEngine;

public class StateMachineState_Player_StandUp : StateMachineState_Player
{
    public StateMachineState_Player_StandUp(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.navmeshAgent.updateRotation = true;
        playerController.currentAnimation.value = PlayerAnimationType.StandUp;
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        rigidBody.velocity = Vector3.zero;
        FixedUpdate_StayGround();
    }

    public override void OnDisable()
    {
        playerController.sittingInteractable.ServerUnsetSittableRPC(NetworkManager.Singleton.LocalClientId);
        //playerController.sittingInteractable.SetCanInteract(true);
        playerController.EnableInteracters();
        playerController.sittingInteractable = null;
        PlayerController.instance.shouldStand = false;
    }
}