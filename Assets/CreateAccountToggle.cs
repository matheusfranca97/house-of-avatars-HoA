using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountToggle : MonoBehaviour
{
    [SerializeField] Toggle myToggle;
    [SerializeField] Button createAccountButton;
    // Start is called before the first frame update
    void Start()
    {
        myToggle.onValueChanged.AddListener(OnToggleValueChange);
    }

    public void OnToggleValueChange(bool value)
    {
        createAccountButton.enabled = value;
    }
}
