public class StateMachineTransition_Player_NavigateToPosition : StateMachineTransition_Player
{
    public StateMachineTransition_Player_NavigateToPosition(StateMachineState_Player targetState)
        : base(targetState) { }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        if (!playerController.shouldNavigate)
            return false;

        return true;
    }
}