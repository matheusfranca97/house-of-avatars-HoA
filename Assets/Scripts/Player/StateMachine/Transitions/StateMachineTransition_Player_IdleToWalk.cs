using UnityEngine;

public class StateMachineTransition_Player_IdleToWalk : StateMachineTransition_Player
{
    public StateMachineTransition_Player_IdleToWalk(StateMachineState_Player targetState)
        : base(targetState) { }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        Vector3 velocity = playerController.rigidBody.velocity;
        velocity.y = 0;
        if (velocity.magnitude < playerController.startMovingStateThreshold || playerController.shouldSit)
            return false;

        return true;
    }
}