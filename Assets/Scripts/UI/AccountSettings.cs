using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityEngine.Rendering.UI;
using Supabase.Gotrue;
using Unity.Netcode;

public class AccountSettings : TaskProcessor
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Color activatedColour;
    [SerializeField] private Color deactivatedColour;
    [SerializeField] private int activatedFontSize;
    [SerializeField] private int deactivatedFontSize;

    private IngameUI ingameUI;
    private List<AccountSettingsButton> pages;
    private int currentPage = 0;
    private User user;
    private GameUser gameUser;

    [SerializeField] private Button backToGameButton;

    [SerializeField] private InputField usernameField;
    [SerializeField] private Button changeUsernameButton;

    [SerializeField] private InputField currentEmailField;
    [SerializeField] private InputField newEmailField;
    [SerializeField] private Button changeEmailButton;

    [SerializeField] private InputField newPasswordField;
    [SerializeField] private Button changePasswordButton;

    [SerializeField] private Button deleteAccountButton;


    private void Start()
    {
        Hide();
        ingameUI = GetComponentInParent<IngameUI>();
        pages = GetComponentsInChildren<AccountSettingsButton>().ToList();

        for (int i=0; i < pages.Count; i++)
        {
            if (i == 0) pages[i].Activate(activatedColour, activatedFontSize);
            else pages[i].DeActivate(deactivatedColour, deactivatedFontSize); 

            int x = i;
            pages[i].button.onClick.AddListener(() => OnPageChange(x));
        }

        //Populate info fields with current user
        if (PlayerSettingsManager.instance.gameUser.value == null)
        {
            throw new NullReferenceException("accountUser.value == null");
        }

        user = SupabaseBridge.instance.SupabaseClient.Auth.CurrentUser;
        gameUser = PlayerSettingsManager.instance.gameUser.value;

        backToGameButton.onClick.AddListener(Close);
        changeUsernameButton.onClick.AddListener(OnUsernameChangeClick);
        changeEmailButton.onClick.AddListener(OnEmailChangeClick);
        changePasswordButton.onClick.AddListener(OnUpdatePasswordClick);
        deleteAccountButton.onClick.AddListener(OnDeleteAccountClick);

        usernameField.onEndEdit.AddListener(OnUsernameFieldChanged);

        usernameField.text = gameUser.Username;
        currentEmailField.text = SupabaseBridge.instance.SupabaseClient.Auth.CurrentUser.Email;

        errorText.gameObject.SetActive(false);
    }

    public void OnPageChange(int pageIndex)
    {
        if (pageIndex == currentPage)
            return;

        pages[currentPage].DeActivate(deactivatedColour, deactivatedFontSize);
        currentPage = pageIndex;
        pages[currentPage].Activate(activatedColour, activatedFontSize);
        errorText.gameObject.SetActive(false);
    }

    private void OnUsernameFieldChanged(string newValue)
    {
        if (newValue == gameUser.Username)
        {
            changeUsernameButton.interactable = false;
        }
        else
        {
            changeUsernameButton.interactable = true;
        }
    }

    private void OnUsernameChangeClick()
    {
        changeUsernameButton.interactable = false;

        ValidationResult validation = ValidationRules.IsUsernameValid(usernameField.text);
        if (!validation.isSuccess)
        {
            Fail(validation.failMessage);
            changeUsernameButton.interactable = true;
            return;
        }
        
        string oldName = gameUser.Username;
        gameUser.Username = usernameField.text;

        Task[] taskList =
        {
            SupabaseBridge.instance.SupabaseClient.From<GameUser>().Update(gameUser),
            SupabaseBridge.instance.SupabaseClient.From<GameUsername>().Delete(new GameUsername(oldName, user.Id))
        };
        StartCoroutine(ProcessTasks(taskList, () =>
        {
            Task reserveUsernameTask = SupabaseBridge.instance.SupabaseClient.From<GameUsername>().Insert(new GameUsername(gameUser.Username, user.Id));
            StartCoroutine(ProcessTasks(taskList, () =>
            {
                Succeed("Username changed successfully");
                changeUsernameButton.interactable = true;
            }, () =>
            {
                Succeed("Username changed successfully, but reserve failed, please contact support");
                changeUsernameButton.interactable = true;
            }));
        }, 
        () => changeUsernameButton.interactable = true));
    }

    private void OnEmailChangeClick()
    {
        //if (changeEmailPasswordField.text == string.Empty)
        //{
        //    Fail("You must enter your current password to change email");
        //}

        if (newEmailField.text == user.Email)
        {
            Fail("Cant change email to the same email");
            return;
        }

        changeEmailButton.interactable = false;
        ValidationResult validation = ValidationRules.IsEmailValid(newEmailField.text);
        if (!validation.isSuccess)
        {
            Fail(validation.failMessage);
            changeEmailButton.interactable = true;
            return;
        }

        UserAttributes attributes = new() { Email = newEmailField.text };
        Task changeTask = SupabaseBridge.instance.SupabaseClient.Auth.Update(attributes);
        StartCoroutine(ProcessTask(changeTask, () =>
        {
            Succeed("An email verification link has been sent to the new email");
            changeEmailButton.interactable = true;
            currentEmailField.text = newEmailField.text;
        }, () => changeEmailButton.interactable = true));
    }

    private void OnUpdatePasswordClick()
    {
        changePasswordButton.interactable = false;

        //Check new password is valid
        ValidationResult validation = ValidationRules.IsPasswordValid(newPasswordField.text);
        if (!validation.isSuccess)
        {
            Fail(validation.failMessage);
            changePasswordButton.interactable = true;
            return;
        }

        //Update password
        UserAttributes attributes = new() { Password = newPasswordField.text };
        Task updatePassword = SupabaseBridge.instance.SupabaseClient.Auth.Update(attributes);
        StartCoroutine(ProcessTask(updatePassword, () =>
        {
            Succeed("Password updated");
            changePasswordButton.interactable = true;
        }, () => changePasswordButton.interactable = true));
    }

    private async void OnDeleteAccountClick()
    {
        deleteAccountButton.interactable = false;
        await SupabaseBridge.instance.SupabaseClient.Auth.SignOut();
        PlayerController.instance.networkController.ServerDeleteUserRPC(NetworkManager.Singleton.LocalClientId);
    }

    private void Close()
    {
        ingameUI.selectedScreenType.value = IngameUIScreenType.None;
    }

    public void Show()
    {
        canvasGroup.Show();
    }

    public void Hide()
    {
        canvasGroup.Hide();
    }
}
