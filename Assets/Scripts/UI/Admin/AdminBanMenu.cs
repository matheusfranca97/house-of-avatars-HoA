using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

public class AdminBanMenu : AdminMenu
{
    public UserAutocompleteField banUserField;
    public UserSelectField banUserSelect;

    public override void Send()
    {
        string targetUsername;
        if (IsUserValid(banUserField.field.text))
            targetUsername = banUserField.field.text;
        else if (IsUserValid(banUserSelect.options[banUserSelect.value].text))
            targetUsername = banUserSelect.options[banUserSelect.value].text;
        else
        {
            SendUserDoesntExist("ban");
            return;
        }

        ulong targetId = PlayerList.instance.GetPlayerIDFromName(targetUsername);
        if (targetId != 0)
        {
            ulong localId = NetworkManager.Singleton.LocalClientId;
            PlayerController.instance.networkController.ServerBanPlayerRPC(localId, targetId);

            banUserField.field.SetTextWithoutNotify("");
            banUserSelect.SetValueWithoutNotify(0);
            closeButton.onClick.Invoke();
        }
    }
}