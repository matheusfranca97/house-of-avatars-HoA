public class StateMachineTransition_Player_FinishSittingDown : StateMachineTransition_Player
{
    private bool eventTriggered;

    public StateMachineTransition_Player_FinishSittingDown(StateMachineState_Player targetState)
        : base(targetState) { }

    public override void OnEnable()
    {
        playerController.eventBridge.onAnimationEvent += OnEvent_Animation;
        eventTriggered = false;
    }

    private void OnEvent_Animation(PlayerCharacterAnimationEvent eventType)
    {
        if (eventType != PlayerCharacterAnimationEvent.SittingEnd)
            return;

        eventTriggered = true;
    }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        if (!eventTriggered)
            return false;

        return true;
    }

    public override void OnDisable()
    {
        playerController.eventBridge.onAnimationEvent -= OnEvent_Animation;
    }
}