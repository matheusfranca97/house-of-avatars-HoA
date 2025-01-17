using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{   
    [SerializeField] public string interactText { get; set; }
    public EventVariable<IInteractable, bool> CanInteract { get; }

    protected Interactable()
    {
        CanInteract = new EventVariable<IInteractable, bool>(this, true);
    }

    public void TriggerInteract()
    {
        Interact();
    }

    protected abstract void Interact();

    private void OnDestroy()
    {
        CanInteract.value = false;
    }
}
