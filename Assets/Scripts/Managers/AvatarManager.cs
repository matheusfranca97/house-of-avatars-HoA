using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AvatarManager : MonoBehaviour
{
    public static AvatarManager instance;

    private void Awake()
    {
        instance = this;
    }

    public PlayerAvatarList avatarList;
    public Sprite serverUserIcon;
}

