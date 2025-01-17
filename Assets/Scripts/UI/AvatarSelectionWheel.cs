using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectionWheel : MonoBehaviour
{
    public readonly EventVariable<AvatarSelectionWheel, short> selectedAvatarIndex;
    [SerializeField] private AvatarOption avatarOptionPrefab;
    [SerializeField] private AvatarOptionDot avatarOptionDotPrefab;
    [SerializeField] private RectTransform avatarOptionsContainer;
    [SerializeField] private RectTransform avatarOptionDotsContainer;
    [SerializeField] private Button previousIndexButton;
    [SerializeField] private Button nextIndexButton;
    [SerializeField] private Button playButton;

    private AvatarSelectionWheel()
    {
        selectedAvatarIndex = new ControlledEventVariable<AvatarSelectionWheel, short>(this, (short)0, Check_SelectedAvatarIndex);
    }

    private short Check_SelectedAvatarIndex(short value)
    {
        if (value < 0)
            return 0;

        if (value >= AvatarManager.instance.avatarList.avatarDataList.Length)
            return (short)(AvatarManager.instance.avatarList.avatarDataList.Length - 1);

        return value;
    }

    private void Awake()
    {
        for (int i = 0; i < AvatarManager.instance.avatarList.avatarDataList.Length; i++)
        {
            PlayerAvatarData playerAvatarData = AvatarManager.instance.avatarList.avatarDataList[i];
            AvatarOption avatarOption = GameObject.Instantiate(avatarOptionPrefab, avatarOptionsContainer);
            avatarOption.Initialize(this, playerAvatarData, i);

            AvatarOptionDot avatarOptionDot = GameObject.Instantiate(avatarOptionDotPrefab, avatarOptionDotsContainer);
            avatarOptionDot.Initialize(this, i);
        }

        previousIndexButton.onClick.AddListener(OnPress_PreviousIndexButton);
        nextIndexButton.onClick.AddListener(OnPress_NextIndexButton);
        playButton.onClick.AddListener(() => StartCoroutine(OnPress_PlayButton()));
    }
    
    private void OnPress_PreviousIndexButton()
    {
        selectedAvatarIndex.value--;
    }

    private void OnPress_NextIndexButton()
    {
        selectedAvatarIndex.value++;
    }

    private IEnumerator OnPress_PlayButton()
    {
        playButton.interactable = false;
        PlayerSettingsManager.instance.playerAvatarData.value = AvatarManager.instance.avatarList.avatarDataList[selectedAvatarIndex.value];
        PlayerSettingsManager.instance.playerAvatarDataIndex.value = selectedAvatarIndex.value;


        Task joinCoroutine = SupabaseBridge.instance.JoinServer();
        while (!joinCoroutine.IsCompleted)
        {
            yield return null;
        }

        if (joinCoroutine.IsFaulted)
        {
            SupabaseBridge.instance.LeaveServer();
            LoginUIPage.instance.Fail(joinCoroutine.Exception);
            StartupUI.instance.selectedScreenType.value = StartupScreenType.MainMenu;
        }

        playButton.interactable = true;
    }

    private void OnDestroy()
    {
        previousIndexButton.onClick.RemoveListener(OnPress_PreviousIndexButton);
        nextIndexButton.onClick.RemoveListener(OnPress_NextIndexButton);
        playButton.onClick.RemoveListener(() => StartCoroutine(OnPress_PlayButton()));
    }
}