using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.UI;

public class AdminKickMenu : AdminMenu
{
    public UserAutocompleteField kickUserField;
    public UserSelectField kickUserSelect;

    public override void Send()
    {
        string targetUsername;
        if (IsUserValid(kickUserField.field.text))
            targetUsername = kickUserField.field.text;
        else if (IsUserValid(kickUserSelect.options[kickUserSelect.value].text))
            targetUsername = kickUserSelect.options[kickUserSelect.value].text;
        else
        {
            SendUserDoesntExist("kick");
            return;
        }

        ulong targetId = PlayerList.instance.GetPlayerIDFromName(targetUsername);

        if (targetId != 0)
        {
            ulong localId = NetworkManager.Singleton.LocalClientId;
            PlayerController.instance.networkController.ServerKickPlayerRPC(localId, targetId);

            kickUserSelect.SetValueWithoutNotify(0);
            kickUserField.field.SetTextWithoutNotify("");
            closeButton.onClick.Invoke();
        }
    }
}
