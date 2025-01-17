
using UnityEngine;
using UnityEngine.UI;

public class AvatarOptionDot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;

    private int index;
    private AvatarSelectionWheel avatarSelectionWheel;

    public void Initialize(AvatarSelectionWheel avatarSelectionWheel, int index)
    {
        this.avatarSelectionWheel = avatarSelectionWheel;
        this.index = index;
        avatarSelectionWheel.selectedAvatarIndex.onValueChangeImmediate += OnValueChanged_AvatarSelectionWheel_SelectedAvatarIndex;
    }

    private void OnValueChanged_AvatarSelectionWheel_SelectedAvatarIndex(short oldValue, short newValue)
    {
        if (newValue == index)
            image.color = selectedColor;
        else
            image.color = unselectedColor;
    }

    private void OnDestroy()
    {
        avatarSelectionWheel.selectedAvatarIndex.onValueChange -= OnValueChanged_AvatarSelectionWheel_SelectedAvatarIndex;
    }
}