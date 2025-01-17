using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AdminToolboxUI : MonoBehaviour
{
    [Header("Toggle Sprites")]
    public Sprite toggleOn;
    public Sprite toggleOff;
    [Header("Toolbox References")]
    public Button toggleButton;
    public Transform adminMenuButtonsContainer;
    private List<AdminMenuButton> menuButtons;

    private int currentMenu = -1; //-1 is none

    private void Start()
    {
        menuButtons = adminMenuButtonsContainer.GetComponentsInChildren<AdminMenuButton>().ToList();

        //Add callbacks
        toggleButton.onClick.AddListener(ToggleToolbox);    
    }

    private void EnableButtons()
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            int x = 0;
            menuButtons[i].onToggledOn.AddListener(() => OpenMenu(x));
            menuButtons[i].onToggledOff.AddListener(() => OpenMenu(-1));
        }
    }

    private void DisableButtons()
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].onToggledOn.RemoveAllListeners();
            menuButtons[i].onToggledOff.RemoveAllListeners();
        }
    }

    private void ToggleToolbox()
    {
        if (adminMenuButtonsContainer.gameObject.activeInHierarchy)
        {
            //Toggle off
            OpenMenu(-1);
            toggleButton.image.sprite = toggleOff;
            adminMenuButtonsContainer.gameObject.SetActive(false);
            DisableButtons();
        }
        else
        {
            //Toggle on
            toggleButton.image.sprite = toggleOn;
            adminMenuButtonsContainer.gameObject.SetActive(true);
            EnableButtons();
        }
    }

    private void OpenMenu(int index)
    {
        if (currentMenu != -1)
        {
            menuButtons[currentMenu].toggle.isOn = false;
        }

        currentMenu = index;
    }
}
