using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerList : NetworkBehaviour
{
    public static PlayerList instance;

    public NetworkList<FixedString512Bytes> playerNameList;
    public NetworkList<ulong> playerIDList;

    private void Awake()
    {
        instance = this;

        playerNameList = new();
        playerIDList = new();
    }

    public ulong GetPlayerIDFromName(string name)
    {
        for (int i=0; i < playerNameList.Count; i++)
        {
            if (playerNameList[i] == name)
            {
                return playerIDList[i];
            }
        }
        return 0;
    }
}