using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UnityEngine;
using UnityEngine.UI;

public class TalkLocalButton : MonoBehaviour
{
	[SerializeField] private Button button;
	private void Start()
	{
		if (PlayerSettingsManager.instance.authLevel.value < PlayerAuthLevel.User)
		{
			gameObject.SetActive(false);
		}

		button.onClick.AddListener(OnPress_Button);
	}

	private void OnPress_Button()
	{
		ChatInput.instance.chatMode.value = ChatMessageType.Local;
	}

	private void OnDestroy()
	{
		button.onClick.RemoveListener(OnPress_Button);
	}
}