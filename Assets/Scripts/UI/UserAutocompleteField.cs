using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UserAutocompleteField : MonoBehaviour
{
    public GameObject fieldViewport;
    public Transform fieldContainer;
    public GameObject fieldPrefab;
    public InputField field;
    List<AutocompleteField> userFields = new();

    protected void Start()
    {
        fieldViewport.SetActive(false);
        field.onValueChanged.AddListener(OnTextChanged);
        field.onSubmit.AddListener(x => ClearUserList());
        field.onEndEdit.AddListener(x => ClearUserList());

        GetComponentInParent<Button>().onClick.AddListener(OnParentClicked);
    }

    public void OnTextChanged(string newText)
    {
        ClearUserList(true);
        PopulateUserList(PlayerList.instance.playerNameList);
    }

    public void PopulateUserList(NetworkList<FixedString512Bytes> users)
    {
        fieldViewport.SetActive(true);
        for (int i = 0; i < users.Count; i++)
        {
            if (!users[i].ToString().StartsWith(field.text) || users[i].ToString() == PlayerSettingsManager.instance.gameUser.value.Username)
            {
                continue;
            }

            AutocompleteField userField = Instantiate(fieldPrefab, fieldContainer).GetComponent<AutocompleteField>();
            userField.text.text = users[i].ToString(); //TO-DO: Set image
            userField.button.onClick.AddListener(() => OnUserFieldClicked(userField));
            userFields.Add(userField);
        }
    }

    public void ClearUserList(bool bypassUICheck = false)
    {
        if (UIUtils.IsPointerOverUIElement(UIUtils.GetEventSystemRaycastResults(), "UI_Autocomplete_Field") && !bypassUICheck)
            return;

        fieldViewport.SetActive(false);
        userFields.ForEach(x => Destroy(x.gameObject));
        userFields.Clear();
    }

    public void OnUserFieldClicked(AutocompleteField userField)
    {
        Debug.Log("deactivating input field");
        field.text = userField.text.text;
        field.DeactivateInputField();

        ClearUserList(true);
        PopulateUserList(PlayerList.instance.playerNameList);
    }

    public void OnParentClicked()
    {
        ClearUserList(true);
        PopulateUserList(PlayerList.instance.playerNameList);
    }
}
