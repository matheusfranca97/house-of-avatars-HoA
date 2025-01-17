using UnityEngine;

public class StateMachineTransition_Player_StandUp : StateMachineTransition_Player
{
    public StateMachineTransition_Player_StandUp(StateMachineState_Player targetState)
        : base(targetState) { }

    private bool shouldTransition;

    public override void OnEnable()
    {
        shouldTransition = false;
    }

    public override bool ShouldTransition_Update(float deltaTime)
    {
        //if ((!Input.GetKeyDown(KeyCode.E) && PlayerSettingsManager.instance.uiFocused.value) || !PlayerController.instance.shouldStand)
        //    return false;

        //shouldTransition = true;
        if ((Input.GetKey(KeyCode.E) && !PlayerSettingsManager.instance.uiFocused.value) || PlayerController.instance.shouldStand)
        {
            shouldTransition = true;
        }
        else
        {
            shouldTransition = false;
        }

        return false;
    }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        if (!shouldTransition)
            return false;

        return true;
    }
}