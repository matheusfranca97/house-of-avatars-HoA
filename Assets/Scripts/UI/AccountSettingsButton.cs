using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountSettingsButton : MonoBehaviour
{
    [SerializeField] public Button button;
    [SerializeField] private Text buttonText;
    [SerializeField] private GameObject sideLine;
    [SerializeField] private CanvasGroup pageGroup;
    [SerializeField] private bool allowGuest = false;

    public void Awake()
    {
        if (PlayerSettingsManager.instance.authLevel.value < PlayerAuthLevel.User && !allowGuest)
        {
            gameObject.SetActive(false);
        }
    }

    public void Activate(Color colour, int fontSize)
    {
        sideLine.SetActive(true);
        ChangeText(colour, fontSize);
        pageGroup.alpha = 1;
        button.interactable = false;
        pageGroup.Show();
    }

    public void DeActivate(Color colour, int fontSize)
    {
        sideLine.SetActive(false);
        ChangeText(colour, fontSize);
        pageGroup.alpha = 0;
        button.interactable = true;
        pageGroup.Hide();
    }

    public void ChangeText(Color colour, int fontSize)
    {
        buttonText.color = colour;
        buttonText.fontSize = fontSize;
    }
}
