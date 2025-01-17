using System;
using UnityEngine;

public class StateMachineState_Player_Sitting : StateMachineState_Player
{
    public StateMachineState_Player_Sitting(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.currentAnimation.value = PlayerAnimationType.Sitting;

        //Turn on UI stand up button
        IngameUI.instance.standUpButton.gameObject.SetActive(true);
        IngameUI.instance.standUpButton.onClick.AddListener(OnStandUpButton);
        playerController.shouldSit = false;
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        rigidBody.velocity = Vector3.zero;
        FixedUpdate_StayGround();
    }

    private void OnStandUpButton()
    {
        playerController.shouldStand = true;
    }

    public override void OnDisable()
    {
        //Turn off UI stand up button
        IngameUI.instance.standUpButton.gameObject.SetActive(false);
        IngameUI.instance.standUpButton.onClick.RemoveListener(OnStandUpButton);
    }
}