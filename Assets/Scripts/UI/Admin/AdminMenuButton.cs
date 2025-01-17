using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdminMenuButton : MonoBehaviour
{
    public class MenuToggledOn : UnityEvent { }
    public class MenuToggledOff : UnityEvent { }

    [SerializeField] public Color normalTextColour = Color.white;
    [SerializeField] public Color pressedTextColour = Color.black;
    [SerializeField] public AdminMenu menu;
    [SerializeField] public Text text;
    [SerializeField] public Toggle toggle;

    public MenuToggledOn onToggledOn = new();
    public MenuToggledOff onToggledOff = new();

    protected void Start()
    {
        toggle.onValueChanged.AddListener(OnToggle);
    }

    public void OnDisable()
    {
        toggle.isOn = false;
        PlayerSettingsManager.instance.uiFocused.value = false;
    }

    public void OnToggle(bool value)
    {
        menu.gameObject.SetActive(value);
        if (value)
        {
            OnToggledOn();
        }
        else
        {
            OnToggledOff();
        }
    }

    public void OnToggledOn()
    {
        text.color = pressedTextColour;
        onToggledOn.Invoke();
        menu.closeButton.onClick.AddListener(CloseButtonClicked);
        PlayerSettingsManager.instance.uiFocused.value = true;
    }

    public void OnToggledOff()
    {
        text.color = normalTextColour;
        onToggledOff.Invoke();
        menu.closeButton.onClick.RemoveListener(CloseButtonClicked);
    }

    public void CloseButtonClicked()
    {
        OnDisable();
    }
}
