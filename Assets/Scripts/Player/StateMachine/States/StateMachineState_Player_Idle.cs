using System;

public class StateMachineState_Player_Idle : StateMachineState_Player
{
    public StateMachineState_Player_Idle(Action initializeFunction)
        : base(initializeFunction) { }

    public override void OnEnable()
    {
        playerController.currentAnimation.value = PlayerAnimationType.Idle;
    }

    public override void Update(float deltaTime)
    {
        if (playerController.characterRotationTransform == null) return;
        RotateCharacterInDirection(GetDesiredCameraRotation());
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        UpdateRigidBodyVelocity(GetDesiredMovementVelocity());
        FixedUpdate_StayGround();
        SetAnimatorSpeeds();
    }
}