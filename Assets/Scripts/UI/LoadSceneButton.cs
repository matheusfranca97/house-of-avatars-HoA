
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private SceneType sceneType;
    [SerializeField] private SpawnLocationType spawnLocationType;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        SceneController.instance.SwitchGameScene(sceneType, spawnLocationType);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}