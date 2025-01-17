
using UnityEngine;
using UnityEngine.UI;

public class SetAccountTypeButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private PlayerAccountType accountType;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        PlayerSettingsManager.instance.accountType.value = accountType;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}