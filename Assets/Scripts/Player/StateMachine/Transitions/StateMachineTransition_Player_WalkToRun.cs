using UnityEngine;

public class StateMachineTransition_Player_WalkToRun : StateMachineTransition_Player
{
    public StateMachineTransition_Player_WalkToRun(StateMachineState_Player targetState)
        : base(targetState) { }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        Vector3 velocity = playerController.rigidBody.velocity;
        velocity.y = 0;
        if (velocity.magnitude < playerController.startRunningStateThreshold || playerController.shouldSit)
            return false;

        return true;
    }
}