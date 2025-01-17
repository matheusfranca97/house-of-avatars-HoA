using Supabase.Gotrue;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIPage : TaskProcessor
{
    public static LoginUIPage instance;

    public InputField emailField;
    public InputField passwordField;
    public Toggle rememberToggle;

    public Button loginButton;
    public Button guestLoginButton;
    public Button forgotPasswordButton;
    public Button exitButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("remember-toggle", 0) == 1)
        {
            emailField.text = Encoding.UTF8.GetString(Convert.FromBase64String(PlayerPrefs.GetString("remember-email", "")));
            passwordField.text = Encoding.UTF8.GetString(Convert.FromBase64String(PlayerPrefs.GetString("remember-password", "")));
            rememberToggle.isOn = true;
        }

        loginButton.onClick.AddListener(() => StartCoroutine(OnLoginClicked()));
        guestLoginButton.onClick.AddListener(() => StartCoroutine(OnGuestLoginClicked()));
        forgotPasswordButton.onClick.AddListener(() => StartCoroutine(OnForgotPasswordClicked()));
        exitButton.onClick.AddListener(OnExitClicked);
    }

    public IEnumerator OnLoginClicked()
    {
        ValidationResult emailValidation = ValidationRules.IsEmailValid(emailField.text);
        ValidationResult passValidation = ValidationRules.IsPasswordValid(passwordField.text);
        if (!emailValidation.isSuccess || !passValidation.isSuccess)
        {
            Fail("Invalid email or password");
            yield break;
        }

        Debug.Log("Trying to login with email");
        if (rememberToggle.isOn)
        {
            //Save email and pass in Base64 encoding
            PlayerPrefs.SetInt("remember-toggle", 1);
            PlayerPrefs.SetString("remember-email", Convert.ToBase64String(Encoding.UTF8.GetBytes(emailField.text)));
            PlayerPrefs.SetString("remember-password", Convert.ToBase64String(Encoding.UTF8.GetBytes(passwordField.text)));
        }
        else
        {
            //Reset remembers
            PlayerPrefs.SetInt("remember-toggle", 0);
            PlayerPrefs.SetString("remember-email", "");
            PlayerPrefs.SetString("remember-password", "");
        }

        Task<Session> loginTask = SupabaseBridge.instance.SupabaseClient.Auth.SignInWithPassword(emailField.text, passwordField.text);
        yield return ProcessTask(loginTask, x => StartCoroutine(OnLoginAuthed(x)));
    }

    private IEnumerator OnLoginAuthed(Session currentSession)
    {
        Debug.Log("Login authorized");
        //Get user data
        Task<GameUser> user = SupabaseBridge.instance.GetGameUser(currentSession);
        yield return ProcessTask(user, x => StartCoroutine(OnLoginGameData(currentSession, x)));
    }

    public IEnumerator OnLoginGameData(Session currentSession, GameUser user)
    {
        if (user == null)
        {
            Fail("Failed to get/create user data, please contact support");
            yield break;
        }
        Debug.Log("Got user data");
        //Check for kick and ban status
        if (user.IsBanned)
        {
            //User is banned
            StartupUI.instance.selectedScreenType.value = StartupScreenType.BannedFromGame;
            yield break;
        }
        else if (user.IsKicked && user.KickedTimestamp != null && user.KickedMinutes != null && ((DateTime)user.KickedTimestamp).AddMinutes((int)user.KickedMinutes) >= DateTime.Now)
        {
            //If the user is past the kick timestamp, log them in and let the server update the value
            StartupUI.instance.selectedScreenType.value = StartupScreenType.KickedFromGame;
            yield break;
        }

        //Login successful
        PlayerSettingsManager.instance.gameUser.value = user;
        PlayerSettingsManager.instance.accountGuid.value = currentSession.User.Id;
        PlayerSettingsManager.instance.authLevel.value = (PlayerAuthLevel)user.AuthLevel;
        StartupUI.instance.selectedScreenType.value = StartupScreenType.AvatarSelection;
        Debug.Log($"User {user.Username} logged in successfully with auth {(PlayerAuthLevel)user.AuthLevel}");
    }

    public IEnumerator OnGuestLoginClicked()
    {
        //Login anonymously
        Debug.Log("Trying to login anonymously (as guest)");
        Task<Session> loginTask = SupabaseBridge.instance.SupabaseClient.Auth.SignInAnonymously();
        yield return ProcessTask(loginTask, x => StartCoroutine(OnGuestAuthed(x)));
    }

    public IEnumerator OnGuestAuthed(Session currentSession)
    {
        //Create game data and feedback to OnLoginGameData
        string guestName = $"Guest {currentSession.User.Id.Substring(0, 4)}";
        Debug.Log($"Guest authorized as {guestName}");
        //Task<GameUser> createDataTask = SupabaseBridge.instance.CreateGameUser(currentSession, guestName, (int)PlayerAuthLevel.Guest);
        //yield return ProcessTask(createDataTask, x => StartCoroutine(OnLoginGameData(currentSession, x)));

        GameUser user = new(currentSession.User.Id, guestName, currentSession.User.CreatedAt);
        user.AuthLevel = (int)PlayerAuthLevel.Guest;

        PlayerSettingsManager.instance.gameUser.value = user;
        PlayerSettingsManager.instance.accountGuid.value = currentSession.User.Id;
        PlayerSettingsManager.instance.authLevel.value = (PlayerAuthLevel)user.AuthLevel;
        StartupUI.instance.selectedScreenType.value = StartupScreenType.AvatarSelection;
        Debug.Log($"Guest {user.Username} logged in successfully with auth {(PlayerAuthLevel)user.AuthLevel}");
        yield return null;
    }

    public IEnumerator OnForgotPasswordClicked()
    {
        ValidationResult validation = ValidationRules.IsEmailValid(emailField.text);
        if (!validation.isSuccess)
        {
            Fail("Invalid Email");
            yield break;
        }

        Task<bool> passResetTask = SupabaseBridge.instance.SupabaseClient.Auth.ResetPasswordForEmail(emailField.text);
        yield return ProcessTask(passResetTask, x => errorText.text = "Password reset link sent to email, check your inbox!");
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }
}
