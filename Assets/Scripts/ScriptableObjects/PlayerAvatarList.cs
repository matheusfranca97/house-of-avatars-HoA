using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAvatarList", menuName = "ScriptableObjects/PlayerAvatarList", order = 2)]
public class PlayerAvatarList : ScriptableObject
{
    [SerializeField] public PlayerAvatarData[] avatarDataList = { };
}
