using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Admin
{
    public class AdminInvisibilityToggle : MonoBehaviour
    {
        public Color normalTextColour = Color.white;
        public Color pressedTextColour = Color.black;
        public Toggle toggle;
        public Text text;

        public void OnEnable()
        {
            toggle.onValueChanged.AddListener(OnToggle);
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnToggle);
        }

        public void OnToggle(bool toggled)
        {
            ulong localId = NetworkManager.Singleton.LocalClientId;
            PlayerController.instance.networkController.ServerToggleInvisibleRPC(localId, !toggled);
        }
    }
}
