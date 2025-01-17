using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public interface IInteractable
{
    public string interactText { get; set; }
    public EventVariable<IInteractable, bool> CanInteract { get; }

    public void TriggerInteract();
}
