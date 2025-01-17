using UnityEngine;

public abstract class Screen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public void Show()
    {
        canvasGroup.Show();
    }

    public void Hide()
    {
        canvasGroup.Hide();
    }
}