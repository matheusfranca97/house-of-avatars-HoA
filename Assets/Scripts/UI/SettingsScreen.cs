using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private SettingSelector fullScreenModeSelector;
    [SerializeField] private SettingSelector resolutionSelector;
    [SerializeField] private SettingSelector fpsLimitSelector;
    [SerializeField] private SettingSelector vsyncSelector;
    [SerializeField] private SettingSelector qualityLevelSelector;

    [SerializeField] private Button resetDefaultButton;
    [SerializeField] private Button applyButton;

    private List<FullScreenMode> fullScreenModes = new()
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };

    private List<Resolution> resolutions;

    private List<int> fpsLimits = new()
    {
        -1,
        10,
        15,
        20,
        30,
        60,
        75,
        90,
        120,
        240,
        360
    };

    private List<string> qualityLevels;

    private void Start()
    {
#if UNITY_WEBGL
        qualityLevelSelector.OnChanged.AddListener(OnOptionChanged);

        fullScreenModeSelector.gameObject.SetActive(false);
        resolutionSelector.gameObject.SetActive(false);
        fpsLimitSelector.gameObject.SetActive(false);
        vsyncSelector.gameObject.SetActive(false);

        applyButton.onClick.AddListener(ApplySettings);
        resetDefaultButton.onClick.AddListener(ResetToDefaultSettings);

        qualityLevels = QualitySettings.names.ToList();
        qualityLevelSelector.AddOptions(qualityLevels.ToArray());
        qualityLevelSelector.SetIndex(PlayerSettingsManager.instance.optionQualityLevel.value);
#else
        ////DEBUG
        //Debug.Log("SETTING FPS");
        //Application.targetFrameRate = 15;

        //Set events
        fullScreenModeSelector.OnChanged.AddListener(OnOptionChanged);
        resolutionSelector.OnChanged.AddListener(OnOptionChanged);

        //Load options for selectors
        resolutions = UnityEngine.Screen.resolutions.Where(x => x.width / x.height == 16 / 9).ToList();

        fullScreenModeSelector.AddOptions(fullScreenModes.Select(x => x.ToString()).ToArray());
        resolutionSelector.AddOptions(resolutions.Select(x => $"{x.width} x {x.height} @ {Math.Round(x.refreshRateRatio.value)}hz").ToArray());
        fpsLimitSelector.AddOptions(fpsLimits.Select(x => x == -1 ? "Off" : x.ToString()).ToArray());
        vsyncSelector.AddOptions(new string[] { "Off", "On" });

        //Load settings from PlayerSettingsManager
        fpsLimitSelector.SetIndex(fpsLimits.IndexOf(PlayerSettingsManager.instance.optionFPSLimit.value));
        vsyncSelector.SetIndex(PlayerSettingsManager.instance.optionVSync.value ? 1 : 0);
        fullScreenModeSelector.SetIndex(fullScreenModes.IndexOf(PlayerSettingsManager.instance.optionFullScreenMode.value));
        try
        {
            //If resolution isn't in the resolution list, add it!
            Resolution playerResolution = PlayerSettingsManager.instance.optionResolution.value;
            if (!resolutions.Any(x => x.width == playerResolution.width && x.height == playerResolution.height && x.refreshRateRatio.value == playerResolution.refreshRateRatio.value))
            {
                resolutions.Add(playerResolution);
                resolutionSelector.AddOptions(new string[] { playerResolution.ToString() });
            }
            resolutionSelector.SetIndex(resolutions.FindIndex(x => x.width == playerResolution.width && x.height == playerResolution.height && x.refreshRateRatio.value == playerResolution.refreshRateRatio.value));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            resolutionSelector.SetIndex(resolutions.Count - 1);
            ApplySettings();
        }
#endif
    }

    private void ApplySettings()
    {
        applyButton.interactable = false;

        //Set the player settings based on the index of the option
#if !UNITY_WEBGL
        PlayerSettingsManager.instance.optionFullScreenMode.value = fullScreenModes[fullScreenModeSelector.CurrentIndex];
        PlayerSettingsManager.instance.optionResolution.value = resolutions[resolutionSelector.CurrentIndex];
        PlayerSettingsManager.instance.optionFPSLimit.value = fpsLimits[fpsLimitSelector.CurrentIndex];
        PlayerSettingsManager.instance.optionVSync.value = vsyncSelector.CurrentIndex == 0 ? false : true;
#else
        PlayerSettingsManager.instance.optionQualityLevel.value = qualityLevelSelector.CurrentIndex;
#endif
    }

    private void ResetToDefaultSettings()
    {
#if !UNITY_WEBGL
        fullScreenModeSelector.SetIndex(1);
        resolutionSelector.SetIndex(resolutions.Count - 1);
        fpsLimitSelector.SetIndex(1);
        vsyncSelector.SetIndex(1);
#else
        qualityLevelSelector.SetIndex(qualityLevels.Count - 1);
#endif

        ApplySettings();
    }

    private void OnOptionChanged()
    {
        applyButton.interactable = true;
    }
}
