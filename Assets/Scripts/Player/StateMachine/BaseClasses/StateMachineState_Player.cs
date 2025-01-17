
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public abstract class StateMachineState_Player : StateMachineState<StateMachineState_Player>
{
    public readonly new StateMachine_Player stateMachine;

    public PlayerController playerController => stateMachine.playerController;
    public Transform transform => playerController.transform;
    public Rigidbody rigidBody => playerController.rigidBody;
    public Animator animator => playerController.animator;
    public CapsuleCollider capsuleCollider => playerController.capsuleCollider;

    public float acceleration => playerController.acceleration;
    public float movementAnimationSpeedFactor => playerController.movementAnimationSpeedFactor;
    public float runAnimationSpeedFactor => playerController.runAnimationSpeedFactor;

    protected StateMachineState_Player(Action initializeFunction)
        : base(initializeFunction)
    {
        stateMachine = StateMachine<StateMachineState_Player>.currentInitializingStateMachine as StateMachine_Player;
    }

    protected Vector3 GetDesiredMovementVelocity()
    {
        if (PlayerSettingsManager.instance.mouseMode.value == MouseMode.UI)
            return Vector3.zero;

        Vector3 desiredVelocity = new Vector3();

        Vector3 forward = playerController.horizontalCameraTransform.forward;
        Vector3 right = playerController.horizontalCameraTransform.right;

        if (Input.GetKey(PlayerController.KEYCODE_FORWARD))
            desiredVelocity += forward;

        if (Input.GetKey(PlayerController.KEYCODE_BACKWARD))
            desiredVelocity -= forward;

        if (Input.GetKey(PlayerController.KEYCODE_RIGHT))
            desiredVelocity += right;

        if (Input.GetKey(PlayerController.KEYCODE_LEFT))
            desiredVelocity -= right;

        desiredVelocity.Normalize();

        if (Input.GetKey(PlayerController.KEYCODE_RUN))
            desiredVelocity *= playerController.maxRunMovementSpeed;
        else
            desiredVelocity *= playerController.maxWalkMovementSpeed;

        return desiredVelocity;
    }

    protected void UpdateRigidBodyVelocity(Vector3 desiredVelocity)
    {
        Vector3 currentVelocity = rigidBody.velocity;
        Vector3 velocityDifference = desiredVelocity - currentVelocity;

        float moveAmount = acceleration * Time.fixedDeltaTime;
        if (moveAmount > velocityDifference.magnitude)
            currentVelocity = desiredVelocity;
        else
            currentVelocity += velocityDifference.normalized * moveAmount;

        rigidBody.velocity = currentVelocity;
    }

    protected void FixedUpdate_StayGround()
    {
        float castDistance = 2;
        if (!CapsuleCastInDirection(Vector3.down, castDistance, out RaycastHit feetRayCastHit))
            return;

        transform.position += Vector3.down * (feetRayCastHit.distance - capsuleCollider.contactOffset * 2);
    }

    private bool CapsuleCastInDirection(Vector3 direction, float distance, out RaycastHit rayCastHit)
    {
        Vector3 centerCapsuleTopSide = transform.position + capsuleCollider.center;
        centerCapsuleTopSide.y += capsuleCollider.height / 2;
        centerCapsuleTopSide.y += capsuleCollider.radius;
        centerCapsuleTopSide -= direction * capsuleCollider.contactOffset * 2;

        Vector3 centerCapsuleBottomSide = transform.position + capsuleCollider.center;
        centerCapsuleBottomSide.y -= capsuleCollider.height / 2;
        centerCapsuleBottomSide.y += capsuleCollider.radius;
        centerCapsuleBottomSide -= direction * capsuleCollider.contactOffset * 2;

        int layerMask = (int)LayerMaskValue.Default;
        layerMask += (int)LayerMaskValue.Floor;
        layerMask += (int)LayerMaskValue.Environment;
        bool result = Physics.CapsuleCast(centerCapsuleTopSide, centerCapsuleBottomSide, capsuleCollider.radius, direction, out rayCastHit, distance + capsuleCollider.contactOffset * 2, layerMask);
        if (rayCastHit.distance <= 0)
            return false;

        rayCastHit.distance -= capsuleCollider.contactOffset * 2;
        return result;
    }

    protected Vector3 GetDesiredCameraRotation()
    {
        if (PlayerSettingsManager.instance.mouseMode.value == MouseMode.UI)
            return playerController.characterRotationTransform.forward;

        return playerController.horizontalCameraTransform.forward;
    }

    protected void RotateCharacterInDirection(Vector3 direction)
    {
        Vector3 forward = playerController.characterRotationTransform.forward;
        direction.y = 0;
        float difference = Vector3.Angle(forward, direction);
        float rotationAmount = (difference * playerController.rotationSpeedFactor + playerController.minimumRotationSpeed) * Time.fixedDeltaTime;

        Vector3 newDirection = Vector3.RotateTowards(forward, direction, rotationAmount * Mathf.Deg2Rad, 0);
        playerController.characterRotationTransform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
    }

    protected void SetAnimatorSpeeds()
    {
        if (playerController.characterRotationTransform == null) return;

        Vector3 moveFactors = rigidBody.velocity;
        moveFactors.y = 0;
        moveFactors = Quaternion.Inverse(playerController.characterRotationTransform.rotation) * moveFactors;
        Vector3 normalizeMoveFactors = moveFactors.normalized;

        List<KeyValuePair<string, float>> speeds = new()
        {
            new(PlayerController.ANIMATOR_X_SPEED, normalizeMoveFactors.x),
            new(PlayerController.ANIMATOR_Y_SPEED, normalizeMoveFactors.z),
            new(PlayerController.ANIMATOR_MOVESPEED, moveFactors.magnitude * movementAnimationSpeedFactor),
            new(PlayerController.ANIMATOR_RUNSPEED, moveFactors.magnitude * runAnimationSpeedFactor)
        };

        foreach (KeyValuePair<string, float> pair in speeds)
        {
            animator.SetFloat(pair.Key, pair.Value);
        }

        if (playerController.networkController != null)
        {
            playerController.networkController.ServerAvatarSetAnimatorSpeedsRPC(NetworkManager.Singleton.LocalClientId, speeds.Select(x => new StringContainer(x.Key)).ToArray(), speeds.Select(x => x.Value).ToArray());
        }
    }
}