using UnityEngine;

public class StateMachineTransition_Player_FinishNavigation : StateMachineTransition_Player
{
    public StateMachineTransition_Player_FinishNavigation(StateMachineState_Player targetState)
        : base(targetState) { }

    public override bool ShouldTransition_FixedUpdate(float fixedDeltaTime)
    {
        if (playerController.cornerIndex < playerController.navMeshPath.corners.Length)
            return false;

        Vector3 difference = playerController.transform.position - playerController.navigatePosition;
        if (difference.magnitude > playerController.finishNavigationThreshold)
            return false;

        playerController.shouldNavigate = false;
        return true;
    }
}