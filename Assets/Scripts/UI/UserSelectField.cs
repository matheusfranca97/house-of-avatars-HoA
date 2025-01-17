using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UserSelectField : Dropdown
{
    protected override void OnEnable()
    {
        base.OnEnable();
        List<string> users = new();
        foreach (FixedString512Bytes user in PlayerList.instance.playerNameList)
        {
            if (user.ToString() == PlayerSettingsManager.instance.gameUser.value.Username)
            {
                continue;
            }
            users.Add(user.ToString());
        }
        AddOptions(users);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ClearOptions();
    }
}
