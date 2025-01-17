using UnityEngine;
using UnityEngine.Rendering.UI;

public class StateMachineTransition_Player_WalkToIdle : StateMachineTransition_Player
{
    public StateMachineTransition_Player_WalkToIdle(StateMachineState_Player targetState)
        : base(targetState) { }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        if (playerController.shouldSit)
        {
            return true;
        }

        Vector3 velocity = playerController.rigidBody.velocity;
        velocity.y = 0;
        if (velocity.magnitude > playerController.stopMovingStateThreshold)
            return false;

        return true;
    }
}