using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

public class AdminMuteMenu : AdminMenu
{
    public UserAutocompleteField muteUserField;
    public UserSelectField muteUserSelect;

    public override void Send()
    {
        string targetUsername;
        if (IsUserValid(muteUserField.field.text))
            targetUsername = muteUserField.field.text;
        else if (IsUserValid(muteUserSelect.options[muteUserSelect.value].text))
            targetUsername = muteUserSelect.options[muteUserSelect.value].text;
        else
        {
            SendUserDoesntExist("mute");
            return;
        }

        ulong targetId = PlayerList.instance.GetPlayerIDFromName(targetUsername);

        if (targetId != 0)
        {
            ulong localId = NetworkManager.Singleton.LocalClientId;
            PlayerController.instance.networkController.ServerMutePlayerRPC(localId, targetId);

            muteUserField.field.SetTextWithoutNotify("");
            muteUserSelect.SetValueWithoutNotify(0);
            closeButton.onClick.Invoke();
        }
    }
}
