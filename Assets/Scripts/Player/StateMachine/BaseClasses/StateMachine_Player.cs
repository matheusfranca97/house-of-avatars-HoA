public class StateMachine_Player : StateMachine<StateMachineState_Player>
{
    public readonly PlayerController playerController;

    private StateMachineState_Player_Idle stateIdle;
    private StateMachineState_Player_SitDown stateSitDown;
    private StateMachineState_Player_Sitting stateSitting;
    private StateMachineState_Player_StandUp stateStandUp;
    private StateMachineState_Player_Walk stateWalk;
    private StateMachineState_Player_Laugh stateLaugh;
    private StateMachineState_Player_Run stateRun;
    private StateMachineState_Player_NavigateToPosition stateNavigateToPosition;

    public StateMachine_Player(PlayerController playerController)
        : base()
    {
        this.playerController = playerController;

        stateIdle = new StateMachineState_Player_Idle(Initialize_Idle);
        stateSitDown = new StateMachineState_Player_SitDown(Initialize_SitDown);
        stateSitting = new StateMachineState_Player_Sitting(Initialize_Sitting);
        stateStandUp = new StateMachineState_Player_StandUp(Initialize_StandUp);
        stateWalk = new StateMachineState_Player_Walk(Initialize_Walk);
        stateLaugh = new StateMachineState_Player_Laugh(Initialize_Laugh);
        stateRun = new StateMachineState_Player_Run(Initialize_Run);
        stateNavigateToPosition = new StateMachineState_Player_NavigateToPosition(Initialize_NavigateToPosition);

        Initialize(stateIdle);
    }

    private void Initialize_Idle()
    {
        new StateMachineTransition_Player_SitDown(stateSitDown);
        new StateMachineTransition_Player_Laugh(stateLaugh);
        new StateMachineTransition_Player_NavigateToPosition(stateNavigateToPosition);
        new StateMachineTransition_Player_IdleToWalk(stateWalk);
    }

    private void Initialize_SitDown()
    {
        new StateMachineTransition_Player_FinishSittingDown(stateSitting);
    }

    private void Initialize_Sitting()
    {
        new StateMachineTransition_Player_StandUp(stateStandUp);
    }

    private void Initialize_StandUp()
    {
        new StateMachineTransition_Player_FinishStandingUp(stateIdle);
    }

    private void Initialize_Walk()
    {
        new StateMachineTransition_Player_SitDown(stateSitDown);
        new StateMachineTransition_Player_Laugh(stateLaugh);
        new StateMachineTransition_Player_NavigateToPosition(stateNavigateToPosition);
        new StateMachineTransition_Player_WalkToIdle(stateIdle);
        new StateMachineTransition_Player_WalkToRun(stateRun);
    }

    private void Initialize_Laugh()
    {
        new StateMachineTransition_Player_FinishLaugh(stateIdle);
    }

    private void Initialize_Run()
    {
        new StateMachineTransition_Player_SitDown(stateSitDown);
        new StateMachineTransition_Player_Laugh(stateLaugh);
        new StateMachineTransition_Player_NavigateToPosition(stateNavigateToPosition);
        new StateMachineTransition_Player_RunToWalk(stateWalk);
    }

    private void Initialize_NavigateToPosition()
    {
        new StateMachineTransition_Player_FinishNavigation(stateIdle);
        new StateMachineTransition_Player_StopNavigation(stateIdle);
    }
}
