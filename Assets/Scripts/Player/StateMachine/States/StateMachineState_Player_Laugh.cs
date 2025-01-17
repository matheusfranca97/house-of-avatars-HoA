using System;
using UnityEngine;

public class StateMachineState_Player_Laugh : StateMachineState_Player
{
    public StateMachineState_Player_Laugh(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.currentAnimation.value = PlayerAnimationType.Laugh;
        playerController.DisableInteracters();
        playerController.shouldLaugh = false;
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        rigidBody.velocity = Vector3.zero;
        FixedUpdate_StayGround();
    }

    public override void OnDisable()
    {
        playerController.EnableInteracters();
    }
}