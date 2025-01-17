using System;
using UnityEngine;

public class StateMachineState_Player_NavigateToPosition : StateMachineState_Player
{
    private float lastDistance;
    public StateMachineState_Player_NavigateToPosition(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.currentAnimation.value = PlayerAnimationType.Walk;
        playerController.DisableInteracters();
        Vector3 difference = playerController.navigatePosition - transform.position;
        lastDistance = difference.magnitude;
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        if (playerController.characterRotationTransform == null) return;

        Vector3 difference = playerController.navigatePosition - transform.position;
        Vector3 desiredVelocity = difference.normalized * playerController.maxWalkMovementSpeed;
        RotateCharacterInDirection(desiredVelocity);
        UpdateRigidBodyVelocity(desiredVelocity);
        FixedUpdate_StayGround();
        SetAnimatorSpeeds();

        if (lastDistance < difference.magnitude)
        {
            playerController.SetNextCorner();
            difference = playerController.navigatePosition - transform.position;
        }

        lastDistance = difference.magnitude;
    }

    public override void OnDisable()
    {
        playerController.EnableInteracters();
    }
}