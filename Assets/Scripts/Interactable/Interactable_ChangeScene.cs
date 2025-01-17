
using UnityEngine;

public class Interactable_ChangeScene : Interactable
{
    [SerializeField] private SceneType sceneType;
    [SerializeField] private SpawnLocationType spawnLocationType;

    protected override void Interact()
    {
        SceneController.instance.SwitchGameScene(sceneType, spawnLocationType);
        CanInteract.value = false;
    }
}