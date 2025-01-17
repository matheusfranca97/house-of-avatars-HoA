
using UnityEngine;
using UnityEngine.AI;

public class AccountTypeContent : MonoBehaviour
{
    [SerializeField] private PlayerAccountType accountType;
    [SerializeField] private GameObject content;

    private void Awake()
    {
        if (PlayerSettingsManager.instance == null) return;
        PlayerSettingsManager.instance.accountType.onValueChangeImmediate += OnValueChanged_AccountType;
    }

    private void OnValueChanged_AccountType(PlayerAccountType oldValue, PlayerAccountType newValue)
    {
        content.SetActive(newValue == accountType);
    }

    private void OnDestroy()
    {
        if (PlayerSettingsManager.instance == null) return;
        PlayerSettingsManager.instance.accountType.onValueChangeImmediate -= OnValueChanged_AccountType;
    }
}