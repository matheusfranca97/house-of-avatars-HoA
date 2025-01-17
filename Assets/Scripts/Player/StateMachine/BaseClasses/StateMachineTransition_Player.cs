
public abstract class StateMachineTransition_Player : StateMachineTransition<StateMachineState_Player>
{
    public readonly new StateMachine_Player stateMachine;
    public PlayerController playerController => stateMachine.playerController;

    protected StateMachineTransition_Player(StateMachineState_Player targetState)
        : base(targetState)
    {
        stateMachine = StateMachine<StateMachineState_Player>.currentInitializingStateMachine as StateMachine_Player;
    }
}