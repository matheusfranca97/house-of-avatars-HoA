using UnityEngine;

public class StateMachineTransition_Player_SitDown : StateMachineTransition_Player
{
    public StateMachineTransition_Player_SitDown(StateMachineState_Player targetState)
        : base(targetState) { }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        if (!playerController.shouldSit)
            return false;

        return true;
    }
}