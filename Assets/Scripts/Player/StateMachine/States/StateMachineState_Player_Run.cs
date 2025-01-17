using System;

public class StateMachineState_Player_Run : StateMachineState_Player
{
    public StateMachineState_Player_Run(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.currentAnimation.value = PlayerAnimationType.Run;
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