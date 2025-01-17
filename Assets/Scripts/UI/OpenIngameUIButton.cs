
using UnityEngine;
using UnityEngine.UI;

public class OpenIngameUIButton : MonoBehaviour
{
    private IngameUI ingameUI;

    [SerializeField] private Button button;
    [SerializeField] private IngameUIScreenType screenType;

    private void Awake()
    {
        gameObject.SetActive(true);
        ingameUI = GetComponentInParent<IngameUI>();
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        ingameUI.selectedScreenType.value = screenType;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}