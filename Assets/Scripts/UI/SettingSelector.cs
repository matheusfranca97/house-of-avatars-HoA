using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingSelector : MonoBehaviour
{
    [SerializeField] private Text optionText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    private List<string> options = new();
    public int CurrentIndex { get; private set; } = 0;

    public UnityEvent OnChanged { get; } = new();

    public void Start()
    {
        nextButton.onClick.AddListener(NextOption);
        previousButton.onClick.AddListener(PreviousOption);
    }

    public void SetIndex(int index)
    {
        CurrentIndex = index;
        ChangeOption(options[CurrentIndex]);
    }

    private void ChangeOption(string option)
    {
        optionText.text = option;
        OnChanged.Invoke();
    }

    public void NextOption()
    {
        if (CurrentIndex + 1 >= options.Count) 
        {
            CurrentIndex = 0;
        }
        else
        {
            CurrentIndex += 1;
        }

        ChangeOption(options[CurrentIndex]);
    }

    public void PreviousOption()
    {
        if (CurrentIndex - 1 < 0)
        {
            CurrentIndex = options.Count - 1;
        }
        else
        {
            CurrentIndex -= 1;
        }

        ChangeOption(options[CurrentIndex]);
    }

    public void AddOptions(string[] newOptions)
    {
        options.AddRange(newOptions);
    }

    public void ClearOptions()
    {
        options.Clear();
        CurrentIndex = 0;
    }
}
