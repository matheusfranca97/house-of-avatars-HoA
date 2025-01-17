using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AdminTeleportMenu : AdminMenu
{
    [SerializeField] private List<AdminTeleportLocation> teleportLocations = new();
    [SerializeField] private Dropdown locationDropdown;

    private void Start()
    {
        //Add dropdown entries
        locationDropdown.AddOptions(teleportLocations.Select(x => new Dropdown.OptionData(x.locationName)).ToList());
    }

    public override void Send()
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;
        AdminTeleportLocation selectedLocation = teleportLocations[locationDropdown.value];
        PlayerController.instance.networkController.ServerTeleportRPC(localId, selectedLocation.position, selectedLocation.rotation, selectedLocation.locationName);
        if (PlayerController.instance.sittingInteractable != null)
        {
            PlayerController.instance.shouldStand = true;
        }
    }
}