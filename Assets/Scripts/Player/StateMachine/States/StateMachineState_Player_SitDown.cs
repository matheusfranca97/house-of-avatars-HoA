using System;
using UnityEngine;

public class StateMachineState_Player_SitDown : StateMachineState_Player
{
    public StateMachineState_Player_SitDown(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.shouldSit = false;
        //playerController.sittingInteractable.SetCanInteract(false);
        playerController.currentAnimation.value = PlayerAnimationType.SitDown;
        playerController.DisableInteracters();
        playerController.navmeshAgent.updateRotation = false;
        playerController.shouldNavigate = false;
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        rigidBody.velocity = Vector3.zero;
        FixedUpdate_StayGround();
        playerController.WarpPosition(playerController.sittingInteractable.sitPosition);
    }

    public override void Update(float deltaTime)
    {
        if (Input.GetKey(KeyCode.E) && !PlayerSettingsManager.instance.uiFocused.value)
        {
            playerController.shouldStand = true;
        }
    }
}