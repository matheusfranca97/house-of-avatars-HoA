
using UnityEngine;
using UnityEngine.UI;

public class GoToGameButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private SceneType gameSceneType;
    [SerializeField] private SpawnLocationType spawnLocation;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        SceneController.instance.LoadGameScene_FromMainMenu(gameSceneType);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}