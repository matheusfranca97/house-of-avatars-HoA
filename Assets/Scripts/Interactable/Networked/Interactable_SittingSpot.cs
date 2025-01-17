using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Interactable_SittingSpot : NetworkInteractable
{
    [field:SerializeField] public Transform sitPosition { private set; get; }
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private Button sitButton;
    [SerializeField] private float buttonDistance = 1;
    [SerializeField] private Vector3 minimumUISize = new Vector3(0.25f, 0.25f, 1f);
    [SerializeField] private Vector3 maxUISize = new Vector3(1, 1, 1);
    [SerializeField] private float referenceDistance = 0.5f;

    private Vector3 originalScale;

    protected override void Interact(bool isTriggerPlayer, bool isGuest)
    {
        if (isTriggerPlayer)
        {
            PlayerController.instance.Sit(this);
            
        }
    }

    private void OnCanInteractChanged(bool oldValue, bool newValue)
    {
        sitButton.gameObject.SetActive(newValue);
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
            CanInteract.onValueChange += OnCanInteractChanged;
            PlayerSettingsManager.instance.cameraMode.onValueChange += OnCameraModeChanged;
            //sitButton.onClick.AddListener(OnSitButtonPressed); HANDLED IN-ENGINE
            originalScale = worldSpaceCanvas.transform.localScale;
            SetEventCamera();
        }
    }

    private void SetEventCamera()
    {
        worldSpaceCanvas.worldCamera = GetActiveCamera();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsClient)
        {
            NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
            CanInteract.onValueChange -= OnCanInteractChanged;
            PlayerSettingsManager.instance.cameraMode.onValueChange -= OnCameraModeChanged;
            //.onClick.RemoveListener(OnSitButtonPressed);
        }
    }

    private void OnNetworkTick()
    {
        if (!CanInteract.value || PlayerController.instance == null)
        {
            return;
        }

        //Check if player is in range to show button
        Camera activeCam = GetActiveCamera();
        float cameraDistance = Vector3.Distance(PlayerController.instance.playerAvatarContainer.transform.position, transform.position);
        if (cameraDistance <= buttonDistance && PlayerController.instance.sittingInteractable == null)
        {
            sitButton.gameObject.SetActive(true);

            //Set button rotation to match -camera angle
            Vector3 rotation = activeCam.transform.eulerAngles;
            rotation.y *= -1;

            worldSpaceCanvas.transform.eulerAngles = activeCam.transform.eulerAngles;

            if (PlayerSettingsManager.instance.cameraMode.value == CameraMode.FirstPerson)
            {
                //Set scale of canvas related to distance between camera and this
                float scaleModifier = cameraDistance / referenceDistance;
                Vector3 newScale = originalScale * scaleModifier;
                newScale.x = Mathf.Clamp(newScale.x, minimumUISize.x * originalScale.x, maxUISize.x * originalScale.x);
                newScale.y = Mathf.Clamp(newScale.y, minimumUISize.y * originalScale.y, maxUISize.y * originalScale.y);
                worldSpaceCanvas.transform.localScale = newScale;
            }
            else
            {
                worldSpaceCanvas.transform.localScale = originalScale;
            }
        }
        else
        {
            sitButton.gameObject.SetActive(false);
        }
    }

    public void OnSitButtonPressed()
    {
        PlayerController.instance.shouldNavigate = false;

        TriggerInteract();
    }

    private void OnCameraModeChanged(CameraMode oldValue, CameraMode newValue)
    {
        SetEventCamera();
    }

    private Camera GetActiveCamera()
    {
        if (PlayerController.instance == null)
        {
            return null;
        }
        return PlayerSettingsManager.instance.cameraMode.value == CameraMode.FirstPerson ? PlayerController.instance.firstPersonCamera : PlayerController.instance.thirdPersonCamera;
    }
}