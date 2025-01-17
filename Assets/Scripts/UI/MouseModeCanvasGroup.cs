
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CanvasGroup))]
public class MouseModeCanvasGroup : MonoBehaviour
{
    [SerializeField] private MouseMode mouseMode;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        PlayerSettingsManager.instance.mouseMode.onValueChangeImmediate += OnValueChanged_MouseMode;
    }

    private void OnValueChanged_MouseMode(MouseMode oldValue, MouseMode newValue)
    {
        if (mouseMode == newValue)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDestroy()
    {
        PlayerSettingsManager.instance.mouseMode.onValueChange -= OnValueChanged_MouseMode;
    }
}