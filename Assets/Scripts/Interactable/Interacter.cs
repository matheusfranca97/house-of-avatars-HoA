
using UnityEngine;

public class Interacter : MonoBehaviour
{
    private const KeyCode INTERACT_KEYCODE = KeyCode.E;
    private const LayerMaskValue INTERACT_LAYER = LayerMaskValue.Interactable;
    private const LayerMaskValue WORLD_LAYER = LayerMaskValue.Default | LayerMaskValue.Environment;

    [SerializeField] private Transform interactLocation;
    [SerializeField] private float maximumInteractDistance;

    private readonly EventVariable<Interacter, IInteractable> currentInteractable;

    private Interacter()
    {
        currentInteractable = new EventVariable<Interacter, IInteractable>(this, default);
    }

    private void Awake()
    {
        currentInteractable.onValueChange += OnValueChanged_CurrentInteractable;
    }

    private void Start()
    {
        PlayerSettingsManager.instance.mouseMode.onValueChangeImmediate += OnValueChanged_MouseMode;
    }

    private void OnValueChanged_MouseMode(MouseMode oldValue, MouseMode newValue)
    {
        if (newValue == MouseMode.UI)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    private void OnValueChanged_CurrentInteractable(IInteractable oldValue, IInteractable newValue)
    {
        if (oldValue != null)
        {
            oldValue.CanInteract.onValueChange -= OnValueChanged_Interactable_CanInteract;
            IngameUI.instance.HideInteractableText();
        }

        if (newValue != null)
        {
            newValue.CanInteract.onValueChange += OnValueChanged_Interactable_CanInteract;
            IngameUI.instance.ShowInteractableText(newValue);
        }
    }

    private void OnValueChanged_Interactable_CanInteract(bool oldValue, bool newValue)
    {
        if (!newValue)
            currentInteractable.value = null;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(interactLocation.position, interactLocation.forward, out hit, maximumInteractDistance, (int)INTERACT_LAYER, QueryTriggerInteraction.Collide) &&
            !Physics.Raycast(interactLocation.position, interactLocation.forward, hit.distance, (int)WORLD_LAYER, QueryTriggerInteraction.Ignore))
        {
            IInteractable interactableComponent = hit.collider.GetComponent<IInteractable>();
            if (interactableComponent.CanInteract.value)
                currentInteractable.value = interactableComponent;
            
        }
        else
            currentInteractable.value = null;

        if (Input.GetKeyUp(INTERACT_KEYCODE) && !PlayerSettingsManager.instance.uiFocused.value && currentInteractable.value != null)
            currentInteractable.value.TriggerInteract();
    }

    private void OnDisable()
    {
        currentInteractable.value = null;
    }

    private void OnDestroy()
    {
        currentInteractable.onValueChange -= OnValueChanged_CurrentInteractable;
        PlayerSettingsManager.instance.mouseMode.onValueChange -= OnValueChanged_MouseMode;
    }
}