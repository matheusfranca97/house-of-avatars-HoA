using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { private set; get; }

    public const KeyCode KEYCODE_FORWARD = KeyCode.W;
    public const KeyCode KEYCODE_BACKWARD = KeyCode.S;
    public const KeyCode KEYCODE_LEFT = KeyCode.A;
    public const KeyCode KEYCODE_RIGHT = KeyCode.D;
    public const KeyCode KEYCODE_RUN = KeyCode.LeftShift;

    public const string ANIMATOR_X_SPEED = "XSpeedFactor";
    public const string ANIMATOR_Y_SPEED = "YSpeedFactor";
    public const string ANIMATOR_MOVESPEED = "Move Speed";
    public const string ANIMATOR_RUNSPEED = "Run Speed";

    public Animator animator;
    public EventBridge_PlayerCharacter eventBridge;
    public SkinnedMeshRenderer[] meshRenderers;
    public Transform characterRotationTransform;
    public NetworkPlayerController networkController;

    public readonly EventVariable<PlayerController, PlayerAnimationType> currentAnimation;

    [SerializeField] private Interacter[] interactors;

    [field: SerializeField] public Transform verticalCameraTransform { private set; get; }
    [field: SerializeField] public Transform horizontalCameraTransform { private set; get; }
    [field: SerializeField] public Camera firstPersonCamera { private set; get; }
    [field: SerializeField] public Camera thirdPersonCamera { private set; get; }

    [field: SerializeField] public Rigidbody rigidBody { private set; get; }
    [field: SerializeField] public CapsuleCollider capsuleCollider { private set; get; }
    [field: SerializeField] public NavMeshAgent navmeshAgent { private set; get; }
    [field: SerializeField] public Transform playerAvatarContainer { set; get; }

    [field: SerializeField] public float movementAnimationSpeedFactor { private set; get; }
    [field: SerializeField] public float runAnimationSpeedFactor { private set; get; }
    [field: SerializeField] public float acceleration { private set; get; }
    [field: SerializeField] public float maxWalkMovementSpeed { private set; get; }
    [field: SerializeField] public float maxRunMovementSpeed { private set; get; }
    [field: SerializeField] public float maxVerticalDegrees { private set; get; }
    [field: SerializeField] public float minVerticalDegrees { private set; get; }
    [field: SerializeField] public float mouseSensitivity { private set; get; }
    [field: SerializeField] public float startMovingStateThreshold { private set; get; }
    [field: SerializeField] public float stopMovingStateThreshold { private set; get; }
    [field: SerializeField] public float startRunningStateThreshold { private set; get; }
    [field: SerializeField] public float stopRunningStateThreshold { private set; get; }
    [field: SerializeField] public float rotationSpeedFactor { private set; get; }
    [field: SerializeField] public float minimumRotationSpeed { private set; get; }
    [field: SerializeField] public float finishNavigationThreshold { private set; get; }


    [NonSerialized] public bool shouldNavigate;
    [NonSerialized] public NavMeshPath navMeshPath;
    [NonSerialized] public int cornerIndex;
    [NonSerialized] public Vector3 navigatePosition;

    [NonSerialized] public bool shouldLaugh;

    [NonSerialized] public bool shouldSit;
    [NonSerialized] public bool shouldStand;
    [NonSerialized] public Interactable_SittingSpot sittingInteractable;

    private float lastAnimationUpdateMoment;
    private StateMachine_Player stateMachine;
    private PlayerAvatar playerAvatar;

    private PlayerController()
    {
        currentAnimation = new EventVariable<PlayerController, PlayerAnimationType>(this, PlayerAnimationType.Idle);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        stateMachine = new StateMachine_Player(this);
        navMeshPath = new NavMeshPath();
        //Avatar container will be parent
        //Colliders, camera, rigidbody, navmesh agent will be components/children of this GameObject
        PlayerSettingsManager.instance.cameraMode.onValueChangeImmediate += OnValueChanged_CameraMode;
    }

    public void SetPlayerAvatar(PlayerAvatar avatar)
    {
        playerAvatar = avatar;
        animator = avatar.animator;
        eventBridge = avatar.eventBridge;
        meshRenderers = avatar.meshRenderers;
        characterRotationTransform = avatar.characterRotationTransform;
        playerAvatarContainer = avatar.transform.parent;
        currentAnimation.onValueChangeImmediate += OnValueChanged_CurrentAnimation;
    }

    private void OnValueChanged_CameraMode(CameraMode oldValue, CameraMode newValue)
    {
        switch(newValue)
        {
            case CameraMode.FirstPerson:
                firstPersonCamera.gameObject.SetActive(true);
                thirdPersonCamera.gameObject.SetActive(false);
                foreach(SkinnedMeshRenderer renderer in meshRenderers)
                    renderer.enabled = false;
                if (playerAvatar == null)
                    break;
                foreach (HideInFirstPerson hideObject in playerAvatar.GetComponentsInChildren<HideInFirstPerson>(includeInactive: true))
                    hideObject.gameObject.SetActive(false);
                break;

            case CameraMode.ThirdPerson:
                thirdPersonCamera.gameObject.SetActive(true);
                firstPersonCamera.gameObject.SetActive(false);
                foreach (SkinnedMeshRenderer renderer in meshRenderers)
                    renderer.enabled = true;
                if (playerAvatar == null)
                    break;
                foreach (HideInFirstPerson hideObject in playerAvatar.GetComponentsInChildren<HideInFirstPerson>(includeInactive: true))
                    hideObject.gameObject.SetActive(true);
                break;
        }
    }

    private void OnValueChanged_CurrentAnimation(PlayerAnimationType oldValue, PlayerAnimationType newValue)
    {
        string oldAnimationTrigger = oldValue.GetIdentifier();
        if (Time.time == lastAnimationUpdateMoment)
        {
            animator.ResetTrigger(oldAnimationTrigger);
        }

        string newAnimationTrigger = newValue.GetIdentifier();
        animator.SetTrigger(newAnimationTrigger);
        networkController.ServerAvatarSetResetTriggersRPC(NetworkManager.Singleton.LocalClientId, Time.time == lastAnimationUpdateMoment ? oldAnimationTrigger : "", newAnimationTrigger);
        lastAnimationUpdateMoment = Time.time;
    }

    private void Update()
    {
        if (networkController == null) return;
        stateMachine.Update(1);
        Update_CameraMovement();
    }

    private void FixedUpdate()
    {
        if (networkController == null) return;
        stateMachine.FixedUpdate(1);
        networkController.UpdateAvatar();
    }

    private void Update_CameraMovement()
    {
        if (!Application.isFocused)
            return;

        float yDifference = 0;
        float xDifference = 0;

        if (PlayerSettingsManager.instance.mouseMode.value == MouseMode.Game)
        {
            yDifference = Mathf.Clamp(Input.GetAxis("Mouse Y"), -3, 3) * mouseSensitivity;
            xDifference = Mathf.Clamp(Input.GetAxis("Mouse X"), -3, 3) * mouseSensitivity;
        }
        else if (PlayerSettingsManager.instance.mouseMode.value == MouseMode.UI && UIPanController.instance != null)
        {
            yDifference = UIPanController.instance.panY * mouseSensitivity;
            xDifference = UIPanController.instance.panX * mouseSensitivity;
        }

        horizontalCameraTransform.Rotate(0, xDifference * (Time.deltaTime * 60f), 0);
        Vector3 cameraAngles = verticalCameraTransform.localRotation.eulerAngles;
        cameraAngles = GetUsableEulerAngles(cameraAngles);
        cameraAngles.x -= yDifference * (Time.deltaTime * 60f);
        cameraAngles.x = Mathf.Clamp(cameraAngles.x, minVerticalDegrees, maxVerticalDegrees);
        verticalCameraTransform.localRotation = Quaternion.Euler(cameraAngles);
    }

    public void DisableInteracters()
    {
        foreach (Interacter interacter in interactors)
            interacter.enabled = false;
    }

    public void EnableInteracters()
    {
        foreach (Interacter interacter in interactors)
            interacter.enabled = true;
    }

    private Vector3 GetUsableEulerAngles(Vector3 angle)
    {
        if (angle.x > 180)
            angle.x -= 360;

        if (angle.y != 180)
            return angle;

        if (angle.z != 180)
            return angle;

        if (angle.x > 0)
            angle.x = 180 - angle.x;
        else
            angle.x = -180 - angle.x;

        angle.y = 0;
        angle.z = 0;
        return angle;
    }

    private void OnDestroy()
    {
        currentAnimation.onValueChange -= OnValueChanged_CurrentAnimation;
        PlayerSettingsManager.instance.cameraMode.onValueChange -= OnValueChanged_CameraMode;
    }

    public void Sit(Interactable_SittingSpot sittingInteractable)
    {
        shouldNavigate = false;
        shouldSit = true;
        this.sittingInteractable = sittingInteractable;
        sittingInteractable.ServerSetSittableRPC(NetworkManager.Singleton.LocalClientId);
    }

    public void TryNavigateToPosition()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = (int) LayerMaskValue.Floor + (int) LayerMaskValue.Environment;
        RaycastHit raycastHit;
        if (!Physics.Raycast(mouseRay, out raycastHit, float.MaxValue, layerMask) || raycastHit.collider.gameObject.layer == (int) LayerMaskValue.Environment)
        {
            shouldNavigate = false;
            return;
        }

        NavMeshHit navMeshHit;
        if (!NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 0.2f, NavMesh.AllAreas))
        {
            shouldNavigate = false;
            return;
        }

        if (!NavMesh.CalculatePath(transform.position, navMeshHit.position, NavMesh.AllAreas, navMeshPath))
        {
            shouldNavigate = false;
            return;
        }

        shouldNavigate = true;
        cornerIndex = 1;
        SetNextCorner();
    }

    public void WarpPosition(Transform transform) => WarpPosition(transform.position, transform.rotation);
    public void WarpPosition(Vector3 position, Quaternion rotation)
    {
        rigidBody.Move(position, rotation);
        transform.SetPositionAndRotation(position, rotation);
        navmeshAgent.Warp(position);
        characterRotationTransform.rotation = rotation;
    }

    public void SetNextCorner()
    {
        if (navMeshPath.corners.Length == cornerIndex)
            return;

        navigatePosition = navMeshPath.corners[cornerIndex];
        cornerIndex++;

    }
}
