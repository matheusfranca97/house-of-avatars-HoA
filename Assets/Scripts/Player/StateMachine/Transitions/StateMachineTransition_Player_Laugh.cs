public class StateMachineTransition_Player_Laugh : StateMachineTransition_Player
{
    public StateMachineTransition_Player_Laugh(StateMachineState_Player targetState)
        : base(targetState) { }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        if (!playerController.shouldLaugh)
            return false;

        return true;
    }
}