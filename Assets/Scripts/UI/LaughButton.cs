
using UnityEngine;
using UnityEngine.UI;

public class LaughButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        PlayerController.instance.shouldLaugh = true;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}