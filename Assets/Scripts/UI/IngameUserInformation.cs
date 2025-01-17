using UnityEngine;
using UnityEngine.UI;

public class IngameUserInformation : MonoBehaviour
{
    [SerializeField] private Text usernameText;
    [SerializeField] private Text userTypeText;
    [SerializeField] private Image userIconImage;

    private void Awake()
    {
        // PlayerSettingsManager.instance.accountType.onValueChangeImmediate += OnValueChanged_AccountType;
        usernameText.text = PlayerSettingsManager.instance.gameUser.value.Username;
        userTypeText.text = PlayerSettingsManager.instance.authLevel.value.ToString();
        userIconImage.sprite = AvatarManager.instance.avatarList.avatarDataList[PlayerSettingsManager.instance.playerAvatarDataIndex.value].avatarChatIcon;
    }

    //private void OnValueChanged_AccountType(PlayerAccountType oldValue, PlayerAccountType newValue)
    //{
    //    if (newValue == PlayerAccountType.Quest)
    //        userTypeText.text = "Guest";
    //    else
    //        userTypeText.text = "User";
    //}

    //private void OnDestroy()
    //{
    //    PlayerSettingsManager.instance.accountType.onValueChange -= OnValueChanged_AccountType;
    //}
}