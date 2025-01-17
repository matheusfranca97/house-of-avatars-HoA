using System;
using System.Numerics;

public class StateMachineState_Player_Walk : StateMachineState_Player
{
    public StateMachineState_Player_Walk(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.currentAnimation.value = PlayerAnimationType.Walk;
    }

    public override void Update(float deltaTime)
    {
        RotateCharacterInDirection(GetDesiredCameraRotation());
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        UpdateRigidBodyVelocity(GetDesiredMovementVelocity());
        FixedUpdate_StayGround();
        SetAnimatorSpeeds();
    }
}