
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerAvatarData", order = 1)]
public class PlayerAvatarData : ScriptableObject
{
    [field: SerializeField] public PlayerAvatar playerAvatar { private set; get; }
    [field: SerializeField] public Sprite avatarSprite { private set; get; }
    [field: SerializeField] public Sprite avatarChatIcon { private set; get; }
}