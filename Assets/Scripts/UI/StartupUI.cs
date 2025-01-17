
using UnityEngine;

public class StartupUI : MonoBehaviour
{
    public static StartupUI instance { private set; get; }
    public readonly EventVariable<StartupUI, StartupScreenType> selectedScreenType;

    private StartupUI()
    {
        selectedScreenType = new EventVariable<StartupUI, StartupScreenType>(this, StartupScreenType.MainMenu);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
}