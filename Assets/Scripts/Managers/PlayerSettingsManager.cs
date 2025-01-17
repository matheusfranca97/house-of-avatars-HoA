using Assets.Scripts.Player;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSettingsManager : MonoBehaviour
{
    private const KeyCode TOGGLE_UI_KEYCODE = KeyCode.Tab;

    public readonly EventVariable<PlayerSettingsManager, FullScreenMode> optionFullScreenMode;
    public readonly EventVariable<PlayerSettingsManager, Resolution> optionResolution;
    public readonly EventVariable<PlayerSettingsManager, bool> optionVSync;
    public readonly EventVariable<PlayerSettingsManager, int> optionFPSLimit;
    public readonly EventVariable<PlayerSettingsManager, int> optionQualityLevel;

    public readonly EventVariable<PlayerSettingsManager, MouseMode> mouseMode;
    public readonly EventVariable<PlayerSettingsManager, CameraMode> cameraMode;
    public readonly EventVariable<PlayerSettingsManager, ChatSizeMode> chatSizeMode;
    public readonly EventVariable<PlayerSettingsManager, PlayerAvatarData> playerAvatarData;
    public readonly EventVariable<PlayerSettingsManager, short> playerAvatarDataIndex;
    public readonly EventVariable<PlayerSettingsManager, PlayerAccountType> accountType;
    public readonly EventVariable<PlayerSettingsManager, GameUser> gameUser;
    public readonly EventVariable<PlayerSettingsManager, string> accountGuid;
    public readonly EventVariable<PlayerSettingsManager, PlayerAuthLevel> authLevel;
    public readonly EventVariable<PlayerSettingsManager, bool> isMuted;
    public readonly EventVariable<PlayerSettingsManager, Vector3> spawnPosition;
    public readonly EventVariable<PlayerSettingsManager, Quaternion> spawnRotation;
    public readonly EventVariable<PlayerSettingsManager, float> chatScrollValue;
    public readonly EventVariable<PlayerSettingsManager, bool> uiFocused;

    [SerializeField] private Texture2D cursorIcon;
    [SerializeField] private int xOffset;
    [SerializeField] private int yOffset;
    private Coroutine adjustCursorRoutine;

    public static PlayerSettingsManager instance { private set; get; }

    private PlayerSettingsManager()
    {
#if UNITY_WEBGL
        optionFullScreenMode = new(this, FullScreenMode.Windowed);
        optionFPSLimit = new(this, 60);
        optionQualityLevel = new(this, 5);
        optionVSync = new(this, true);
#else
        optionFullScreenMode = new(this, FullScreenMode.FullScreenWindow);
        optionResolution = new(this, new Resolution());
        optionFPSLimit = new(this, 0);
        optionQualityLevel = new(this, 0);
        optionVSync = new(this, true);
#endif

        mouseMode = new EventVariable<PlayerSettingsManager, MouseMode>(this, MouseMode.UI);
        cameraMode = new EventVariable<PlayerSettingsManager, CameraMode>(this, CameraMode.ThirdPerson);
        chatSizeMode = new EventVariable<PlayerSettingsManager, ChatSizeMode>(this, ChatSizeMode.Maximized);
        playerAvatarData = new EventVariable<PlayerSettingsManager, PlayerAvatarData>(this, null);
        playerAvatarDataIndex = new EventVariable<PlayerSettingsManager, short>(this, 0);
        accountType = new EventVariable<PlayerSettingsManager, PlayerAccountType>(this, default);
        gameUser = new EventVariable<PlayerSettingsManager, GameUser>(this, default);
        accountGuid = new EventVariable<PlayerSettingsManager, string>(this, default);
        authLevel = new EventVariable<PlayerSettingsManager, PlayerAuthLevel>(this, PlayerAuthLevel.Guest);
        isMuted = new EventVariable<PlayerSettingsManager, bool>(this, false);
        spawnPosition = new(this, Vector3.zero);
        spawnRotation = new(this, Quaternion.identity);
        chatScrollValue = new(this, 0);
        uiFocused = new(this, false);
    }

    public SpawnLocationType GetSpawnLocation()
    {
        if (authLevel.value == PlayerAuthLevel.Guest)
            return SpawnLocationType.LoginGuest;

        return SpawnLocationType.LoginAccount;
    }

    private void Awake()
    {
        instance = this;
        mouseMode.onValueChangeImmediate += OnValueChanged_MouseMode;

        optionFullScreenMode.onValueChange += OnValueChanged_OptionFullScreenMode;
        optionFPSLimit.onValueChange += OnValueChanged_OptionFPSLimit;
        optionQualityLevel.onValueChange += OnValueChanged_OptionQualityLevel;
        optionVSync.onValueChange += OnValueChanged_OptionVSync;

        cameraMode.value = (CameraMode)PlayerPrefs.GetInt("cameraMode", 1);

        optionFullScreenMode.TriggerOnValueChange(this, FullScreenMode.Windowed);
        optionQualityLevel.TriggerOnValueChange(this, PlayerPrefs.GetInt("optionQualityLevel", QualitySettings.names.Length - 1), true);
        optionVSync.TriggerOnValueChange(this, PlayerPrefs.GetInt("optionVSync", 1) == 0 ? false : true, true);
        optionFPSLimit.TriggerOnValueChange(this, PlayerPrefs.GetInt("optionFPSLimit", 60), true);

#if !UNITY_WEBGL
        //Load player settings from PlayerPrefs, we do this here instead of the constructor
        //to trigger OnValueChanged to set the settings in the engine
        
        optionResolution.onValueChange += OnValueChanged_OptionResolution;

        optionFullScreenMode.TriggerOnValueChange(this, (FullScreenMode)PlayerPrefs.GetInt("optionFullScreenMode", 1), true);
        string savedRes = PlayerPrefs.GetString("optionResolution", "");
        string[] savedResSplit = savedRes.Split(" ");
        Resolution optionRes = UnityEngine.Screen.currentResolution;
        try
        {
            if (savedResSplit.Length == 5)
            {
                optionRes = UnityEngine.Screen.resolutions.FirstOrDefault(x =>
                    x.width == int.Parse(savedResSplit[0]) &&
                    x.height == int.Parse(savedResSplit[2]) &&
                    x.refreshRateRatio.value == double.Parse(savedResSplit[4].Substring(0, savedResSplit[4].Length - 2)));
            }
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);   
        }
        optionResolution.TriggerOnValueChange(this, optionRes, true);
#endif
    }

    private void Update()
    {
        if (Input.GetKeyUp(TOGGLE_UI_KEYCODE) && !uiFocused.value && StartupUI.instance == null)
        {
            IngameUI.instance?.SaveChatScroll();
            mouseMode.value = mouseMode.value == MouseMode.UI ? MouseMode.Game : MouseMode.UI;
        }
    }

    private void OnValueChanged_MouseMode(MouseMode oldValue, MouseMode newValue)
    {
        switch(newValue)
        {
            case MouseMode.Game:
                Cursor.visible = false;
                if (Application.isFocused)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                
                break;
            case MouseMode.UI:
                if (adjustCursorRoutine != null)
                {
                    StopCoroutine(adjustCursorRoutine);
                    adjustCursorRoutine = null;
                }
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Vector2 hotSpot = new Vector2(xOffset, yOffset);
                Cursor.SetCursor(cursorIcon, hotSpot, CursorMode.ForceSoftware);
                break;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (mouseMode.value is MouseMode.Game)
        {
            Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private void OnValueChanged_OptionFullScreenMode(FullScreenMode oldValue, FullScreenMode newValue)
    {
        UnityEngine.Screen.fullScreenMode = newValue;
        PlayerPrefs.SetInt("optionFullScreenMode", (int)newValue);
        PlayerPrefs.Save();
    }

    private void OnValueChanged_OptionResolution(Resolution oldValue, Resolution newValue)
    {
        UnityEngine.Screen.SetResolution(newValue.width, newValue.height, optionFullScreenMode.value);
        PlayerPrefs.SetString("optionResolution", newValue.ToString());
        PlayerPrefs.Save();
    }

    private void OnValueChanged_OptionVSync(bool oldValue, bool newValue)
    {
        QualitySettings.vSyncCount = newValue ? 1 : 0;
        PlayerPrefs.SetInt("optionVSync", newValue ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void OnValueChanged_OptionFPSLimit(int oldValue, int newValue)
    {
        Application.targetFrameRate = newValue;
        PlayerPrefs.SetInt("optionFPSLimit", newValue);
        PlayerPrefs.Save();
    }

    private void OnValueChanged_OptionQualityLevel(int oldValue, int newValue)
    {
        QualitySettings.SetQualityLevel(newValue);
        PlayerPrefs.SetInt("optionQualityLevel", newValue);
        PlayerPrefs.Save();
        QualitySettings.vSyncCount = optionVSync.value ? 1 : 0;
    }

    private void OnDestroy()
    {
        mouseMode.onValueChange -= OnValueChanged_MouseMode;
        PlayerPrefs.SetInt("cameraMode", (int)cameraMode.value);
    }
}