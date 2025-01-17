using Supabase.Gotrue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountUIPage : TaskProcessor
{
    public InputField userField;
    public InputField emailField;
    public InputField passField;

    public Button createButton;
    public Button cancelButton;

    private void Start()
    {
        createButton.onClick.AddListener(() => StartCoroutine(OnCreateClicked()));
        cancelButton.onClick.AddListener(() => StartCoroutine(OnCancelClicked()));
    }

    public IEnumerator OnCreateClicked()
    {
        ValidationResult emailValidation = ValidationRules.IsEmailValid(emailField.text);
        if (!emailValidation.isSuccess)
        {
            Fail(emailValidation.failMessage);
            yield break;
        }

        ValidationResult passwordValidation = ValidationRules.IsPasswordValid(passField.text);
        if (!passwordValidation.isSuccess)
        {
            Fail(passwordValidation.failMessage);
            yield break;
        }

        ValidationResult userValidation = ValidationRules.IsUsernameValid(userField.text);
        if (!userValidation.isSuccess)
        {
            Fail(userValidation.failMessage);
            yield break;
        }

        //Check username isn't taken
        Task<bool> userTaken = SupabaseBridge.instance.IsUsernameTaken(userField.text);
        yield return new WaitUntil(() => userTaken.IsCompleted);
        if (userTaken.Result)
        {
            Fail("Username is already taken");
            yield break;
        }

        Debug.Log($"Trying to create account with username {userField.text}");
        Task<Session> createAccountTask = SupabaseBridge.instance.SupabaseClient.Auth.SignUp(emailField.text, passField.text);
        yield return ProcessTask(createAccountTask, x => StartCoroutine(OnNewAccountMade(x)));
    }

    public IEnumerator OnNewAccountMade(Session session)
    {
        Task<Session> signInTask = SupabaseBridge.instance.SupabaseClient.Auth.SetSession(session.AccessToken, session.RefreshToken);
        yield return ProcessTask(signInTask, x => StartCoroutine(OnNewAccountAuthorized(x)));
    }

    public IEnumerator OnNewAccountAuthorized(Session currentSession)
    {
        Debug.Log("New account authorized");
        Task<GameUser> createDataTask = SupabaseBridge.instance.CreateGameUser(currentSession, userField.text, (int)PlayerAuthLevel.User);
        yield return ProcessTask(createDataTask, x => 
        { 
            Debug.Log($"User {userField.text} created successfully"); 
            StartCoroutine(ReserveUsername(x)); 
            StartCoroutine(LoginUIPage.instance.OnLoginGameData(currentSession, x)); 
        });
    }

    public IEnumerator ReserveUsername(GameUser user)
    {
        Task reserveTask = SupabaseBridge.instance.SupabaseClient
            .From<GameUsername>()
            .Where(x => x.Id == user.Id)
            .Insert(new GameUsername(user.Username, user.Id));
        yield return ProcessTask(reserveTask);
    }

    public IEnumerator OnCancelClicked()
    {
        userField.text = "";
        emailField.text = "";
        passField.text = "";
        StartupUI.instance.selectedScreenType.value = StartupScreenType.MainMenu;
        yield break;
    }

    private void ToggleButtons(bool toggled)
    {
        createButton.interactable = toggled;
        cancelButton.interactable = toggled;
    }
}